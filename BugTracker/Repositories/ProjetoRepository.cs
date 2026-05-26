using Microsoft.EntityFrameworkCore;
using BugTracker.Data;
using BugTracker.Models;

namespace BugTracker.Repositories;

public class ProjetoRepository : IProjetoRepository
{
    private readonly AppDbContext _context;
    public ProjetoRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Projeto>> GetAllAsync() =>
        await _context.Projetos.Include(p => p.Bugs).ToListAsync();

    public async Task<Projeto?> GetByIdAsync(int id) =>
        await _context.Projetos.Include(p => p.Bugs).FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Projeto> CreateAsync(Projeto projeto)
    {
        _context.Projetos.Add(projeto);
        await _context.SaveChangesAsync();
        return projeto;
    }

    public async Task<Projeto> UpdateAsync(Projeto projeto)
    {
        _context.Projetos.Update(projeto);
        await _context.SaveChangesAsync();
        return projeto;
    }

    public async Task DeleteAsync(Projeto projeto)
    {
        _context.Projetos.Remove(projeto);
        await _context.SaveChangesAsync();
    }
}
