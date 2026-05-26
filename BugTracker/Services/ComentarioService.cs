using BugTracker.DTOs;
using BugTracker.Models;
using BugTracker.Repositories;

namespace BugTracker.Services;

public class ComentarioService : IComentarioService
{
    private readonly IComentarioRepository _repo;
    public ComentarioService(IComentarioRepository repo) => _repo = repo;

    public async Task<IEnumerable<ComentarioResponseDTO>> GetByBugIdAsync(int bugId)
    {
        var comentarios = await _repo.GetByBugIdAsync(bugId);
        return comentarios.Select(ToDTO);
    }

    public async Task<ComentarioResponseDTO> CreateAsync(int bugId, ComentarioCreateDTO dto, int usuarioId)
    {
        var comentario = new Comentario
        {
            BugId = bugId,
            UsuarioId = usuarioId,
            Texto = dto.Texto
        };
        var created = await _repo.CreateAsync(comentario);
        return ToDTO(created);
    }

    private static ComentarioResponseDTO ToDTO(Comentario c) => new()
    {
        Id = c.Id,
        Texto = c.Texto,
        CriadoEm = c.CriadoEm,
        UsuarioId = c.UsuarioId,
        UsuarioNome = c.Usuario?.Nome ?? "",
        UsuarioPerfil = c.Usuario?.Perfil ?? ""
    };
}
