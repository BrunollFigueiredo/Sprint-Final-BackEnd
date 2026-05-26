using BugTracker.DTOs;

namespace BugTracker.Services;

public interface IProjetoService
{
    Task<IEnumerable<ProjetoResponseDTO>> GetAllAsync();
    Task<ProjetoResponseDTO?> GetByIdAsync(int id);
    Task<ProjetoResponseDTO> CreateAsync(ProjetoCreateDTO dto);
    Task<ProjetoResponseDTO?> UpdateAsync(int id, ProjetoUpdateDTO dto);
    Task<bool> DeleteAsync(int id);
}
