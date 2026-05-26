using BugTracker.DTOs;

namespace BugTracker.Services;

public interface IUsuarioService
{
    Task<IEnumerable<UsuarioResponseDTO>> GetAllAsync();
    Task<UsuarioResponseDTO?> GetByIdAsync(int id);
    Task<UsuarioResponseDTO?> UpdateAsync(int id, UsuarioUpdateDTO dto);
    Task<bool> DeleteAsync(int id);
}
