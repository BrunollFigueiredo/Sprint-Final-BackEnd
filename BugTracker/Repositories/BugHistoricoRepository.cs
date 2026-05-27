using Microsoft.EntityFrameworkCore;
using BugTracker.Data;
using BugTracker.Models;

namespace BugTracker.Repositories;

public class BugHistoricoRepository : IBugHistoricoRepository
{
    private readonly AppDbContext _db;
    public BugHistoricoRepository(AppDbContext db) => _db = db;

    public Task<List<BugHistorico>> GetByBugIdAsync(int bugId) =>
        _db.BugHistoricos
            .Include(h => h.Usuario)
            .Where(h => h.BugId == bugId)
            .OrderBy(h => h.CriadoEm)
            .ToListAsync();

    public async Task CreateAsync(BugHistorico historico)
    {
        _db.BugHistoricos.Add(historico);
        await _db.SaveChangesAsync();
    }
}
