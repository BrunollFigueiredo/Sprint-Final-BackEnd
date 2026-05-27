using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BugTracker.Data;
using BugTracker.DTOs;
using BugTracker.Models;
using BugTracker.Services;

namespace BugTracker.Controllers;

[ApiController]
[Route("api/bugs/{bugId}/media")]
[Authorize]
public class BugMediaController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ISupabaseStorageService _storage;

    private static readonly string[] AllowedTypes =
        ["image/jpeg", "image/png", "image/gif", "image/webp", "video/mp4", "video/webm"];
    private const long MaxImageBytes = 5L * 1024 * 1024;
    private const long MaxVideoBytes = 50L * 1024 * 1024;
    private const int MaxFilesPerBug = 5;

    public BugMediaController(AppDbContext context, ISupabaseStorageService storage)
    {
        _context = context;
        _storage = storage;
    }

    [HttpPost]
    [RequestSizeLimit(55 * 1024 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = 55 * 1024 * 1024)]
    public async Task<IActionResult> Upload(int bugId, IFormFileCollection files)
    {
        var bug = await _context.Bugs.FindAsync(bugId);
        if (bug == null) return NotFound(new { mensagem = "Bug não encontrado." });

        if (!files.Any()) return BadRequest(new { mensagem = "Nenhum arquivo enviado." });

        var existingCount = await _context.BugMedias.CountAsync(m => m.BugId == bugId);
        if (existingCount + files.Count > MaxFilesPerBug)
            return BadRequest(new { mensagem = $"Limite de {MaxFilesPerBug} arquivos por bug atingido." });

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var uploaded = new List<BugMediaDTO>();

        foreach (var file in files)
        {
            if (!AllowedTypes.Contains(file.ContentType))
                return BadRequest(new { mensagem = $"Tipo '{file.ContentType}' não permitido. Aceitos: JPG, PNG, GIF, WebP, MP4, WebM." });

            var isVideo = file.ContentType.StartsWith("video/");
            var maxSize = isVideo ? MaxVideoBytes : MaxImageBytes;
            if (file.Length > maxSize)
                return BadRequest(new { mensagem = $"'{file.FileName}' excede o limite ({(isVideo ? "50MB" : "5MB")})." });

            var (storagePath, publicUrl) = await _storage.UploadAsync(file, bugId);

            var media = new BugMedia
            {
                BugId = bugId,
                NomeOriginal = file.FileName,
                StoragePath = storagePath,
                Url = publicUrl,
                ContentType = file.ContentType,
                TamanhoBytes = file.Length,
                UploadedByUserId = userId
            };

            _context.BugMedias.Add(media);
            await _context.SaveChangesAsync();

            uploaded.Add(new BugMediaDTO
            {
                Id = media.Id,
                NomeOriginal = media.NomeOriginal,
                Url = media.Url,
                ContentType = media.ContentType,
                TamanhoBytes = media.TamanhoBytes,
                UploadedByUserId = media.UploadedByUserId,
                CriadoEm = media.CriadoEm
            });
        }

        return Ok(uploaded);
    }

    [HttpDelete("{mediaId}")]
    public async Task<IActionResult> Delete(int bugId, int mediaId)
    {
        var media = await _context.BugMedias
            .FirstOrDefaultAsync(m => m.Id == mediaId && m.BugId == bugId);
        if (media == null) return NotFound(new { mensagem = "Mídia não encontrada." });

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (!User.IsInRole("Admin") && media.UploadedByUserId != userId)
            return Forbid();

        await _storage.DeleteAsync(media.StoragePath);
        _context.BugMedias.Remove(media);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
