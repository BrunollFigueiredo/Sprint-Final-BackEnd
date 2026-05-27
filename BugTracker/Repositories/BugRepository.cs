using Microsoft.EntityFrameworkCore;
using BugTracker.Data;
using BugTracker.Models;

namespace BugTracker.Repositories;

public class BugRepository : IBugRepository
{
    private readonly AppDbContext _context;
    public BugRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Bug>> GetAllAsync() =>
        await _context.Bugs
            .Include(b => b.Projeto)
            .Include(b => b.ReportadoPor)
            .Include(b => b.AtribuidoPara)
            .Include(b => b.BugTags).ThenInclude(bt => bt.Tag)
            .ToListAsync();

    public async Task<Bug?> GetByIdAsync(int id) =>
        await _context.Bugs
            .Include(b => b.Projeto)
            .Include(b => b.ReportadoPor)
            .Include(b => b.AtribuidoPara)
            .Include(b => b.BugTags).ThenInclude(bt => bt.Tag)
            .FirstOrDefaultAsync(b => b.Id == id);

    public async Task<IEnumerable<Bug>> GetByProjetoIdAsync(int projetoId) =>
        await _context.Bugs
            .Include(b => b.Projeto)
            .Include(b => b.ReportadoPor)
            .Include(b => b.AtribuidoPara)
            .Include(b => b.BugTags).ThenInclude(bt => bt.Tag)
            .Where(b => b.ProjetoId == projetoId)
            .ToListAsync();

    public async Task<Bug> CreateAsync(Bug bug)
    {
        _context.Bugs.Add(bug);
        await _context.SaveChangesAsync();
        return bug;
    }

    public async Task<Bug> UpdateAsync(Bug bug)
    {
        _context.Bugs.Update(bug);
        await _context.SaveChangesAsync();
        return bug;
    }

    public async Task DeleteAsync(Bug bug)
    {
        _context.Bugs.Remove(bug);
        await _context.SaveChangesAsync();
    }
}
