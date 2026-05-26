using BugTracker.Models;

namespace BugTracker.Repositories;

public interface IBugRepository
{
    Task<IEnumerable<Bug>> GetAllAsync();
    Task<Bug?> GetByIdAsync(int id);
    Task<IEnumerable<Bug>> GetByProjetoIdAsync(int projetoId);
    Task<Bug> CreateAsync(Bug bug);
    Task<Bug> UpdateAsync(Bug bug);
    Task DeleteAsync(Bug bug);
}
