using Microsoft.EntityFrameworkCore;
using BugTracker.Data;
using BugTracker.Models;

namespace BugTracker.Repositories;

public class ComentarioRepository : IComentarioRepository
{
    private readonly AppDbContext _db;
    public ComentarioRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<Comentario>> GetByBugIdAsync(int bugId) =>
        await _db.Comentarios
            .Include(c => c.Usuario)
            .Where(c => c.BugId == bugId)
            .OrderBy(c => c.CriadoEm)
            .ToListAsync();

    public async Task<Comentario> CreateAsync(Comentario comentario)
    {
        _db.Comentarios.Add(comentario);
        await _db.SaveChangesAsync();
        await _db.Entry(comentario).Reference(c => c.Usuario).LoadAsync();
        return comentario;
    }
}
