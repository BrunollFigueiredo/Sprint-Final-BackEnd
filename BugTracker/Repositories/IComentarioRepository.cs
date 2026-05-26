using BugTracker.Models;

namespace BugTracker.Repositories;

public interface IComentarioRepository
{
    Task<IEnumerable<Comentario>> GetByBugIdAsync(int bugId);
    Task<Comentario> CreateAsync(Comentario comentario);
}
