using BugTracker.Models;

namespace BugTracker.Repositories;

public interface IBugHistoricoRepository
{
    Task<List<BugHistorico>> GetByBugIdAsync(int bugId);
    Task CreateAsync(BugHistorico historico);
}
