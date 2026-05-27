using Microsoft.EntityFrameworkCore;
using BugTracker.Data;
using BugTracker.Models;

namespace BugTracker.Repositories;

public class TagRepository : ITagRepository
{
    private readonly AppDbContext _db;
    public TagRepository(AppDbContext db) => _db = db;

    public Task<List<Tag>> GetAllAsync() =>
        _db.Tags.OrderBy(t => t.Departamento).ThenBy(t => t.Nome).ToListAsync();

    public Task<Tag?> GetByIdAsync(int id) =>
        _db.Tags.FirstOrDefaultAsync(t => t.Id == id);

    public Task<List<Tag>> GetByIdsAsync(List<int> ids) =>
        _db.Tags.Where(t => ids.Contains(t.Id)).ToListAsync();

    public async Task<Tag> CreateAsync(Tag tag)
    {
        _db.Tags.Add(tag);
        await _db.SaveChangesAsync();
        return tag;
    }

    public async Task DeleteAsync(Tag tag)
    {
        _db.Tags.Remove(tag);
        await _db.SaveChangesAsync();
    }
}
