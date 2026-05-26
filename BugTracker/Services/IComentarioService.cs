using BugTracker.DTOs;

namespace BugTracker.Services;

public interface IComentarioService
{
    Task<IEnumerable<ComentarioResponseDTO>> GetByBugIdAsync(int bugId);
    Task<ComentarioResponseDTO> CreateAsync(int bugId, ComentarioCreateDTO dto, int usuarioId);
}
