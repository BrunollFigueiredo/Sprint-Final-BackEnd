using BugTracker.DTOs;
using BugTracker.Models;
using BugTracker.Repositories;

namespace BugTracker.Services;

public class TagService : ITagService
{
    private readonly ITagRepository _repo;
    public TagService(ITagRepository repo) => _repo = repo;

    public async Task<List<TagResponseDTO>> GetAllAsync() =>
        (await _repo.GetAllAsync()).Select(ToDTO).ToList();

    public async Task<TagResponseDTO> CreateAsync(TagCreateDTO dto)
    {
        var tag = new Tag
        {
            Nome = dto.Nome,
            Cor = dto.Cor,
            Departamento = dto.Departamento
        };
        var created = await _repo.CreateAsync(tag);
        return ToDTO(created);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var tag = await _repo.GetByIdAsync(id);
        if (tag == null) return false;
        await _repo.DeleteAsync(tag);
        return true;
    }

    private static TagResponseDTO ToDTO(Tag t) => new()
    {
        Id = t.Id,
        Nome = t.Nome,
        Cor = t.Cor,
        Departamento = t.Departamento
    };
}
