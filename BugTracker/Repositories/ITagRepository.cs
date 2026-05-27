using BugTracker.Models;

namespace BugTracker.Repositories;

public interface ITagRepository
{
    Task<List<Tag>> GetAllAsync();
    Task<Tag?> GetByIdAsync(int id);
    Task<List<Tag>> GetByIdsAsync(List<int> ids);
    Task<Tag> CreateAsync(Tag tag);
    Task DeleteAsync(Tag tag);
}
