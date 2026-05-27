using BugTracker.DTOs;

namespace BugTracker.Services;

public interface ITagService
{
    Task<List<TagResponseDTO>> GetAllAsync();
    Task<TagResponseDTO> CreateAsync(TagCreateDTO dto);
    Task<bool> DeleteAsync(int id);
}
