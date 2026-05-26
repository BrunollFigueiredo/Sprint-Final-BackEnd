using BugTracker.DTOs;

namespace BugTracker.Services;

public interface IBugService
{
    Task<IEnumerable<BugResponseDTO>> GetAllAsync();
    Task<BugResponseDTO?> GetByIdAsync(int id);
    Task<IEnumerable<BugResponseDTO>> GetByProjetoIdAsync(int projetoId);
    Task<BugResponseDTO> CreateAsync(BugCreateDTO dto, int reportadoPorId);
    Task<BugResponseDTO?> UpdateAsync(int id, BugUpdateDTO dto, int userId, bool isAdmin);
    Task<bool> DeleteAsync(int id);
}
