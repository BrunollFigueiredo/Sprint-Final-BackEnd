using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BugTracker.DTOs;
using BugTracker.Services;

namespace BugTracker.Controllers;

[ApiController]
[Route("api/bugs/{bugId}/comentarios")]
[Authorize]
public class ComentariosController : ControllerBase
{
    private readonly IComentarioService _service;
    public ComentariosController(IComentarioService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll(int bugId) =>
        Ok(await _service.GetByBugIdAsync(bugId));

    [HttpPost]
    public async Task<IActionResult> Create(int bugId, [FromBody] ComentarioCreateDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _service.CreateAsync(bugId, dto, usuarioId);
        return CreatedAtAction(nameof(GetAll), new { bugId }, result);
    }
}
