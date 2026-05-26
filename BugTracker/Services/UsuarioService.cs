using BugTracker.DTOs;
using BugTracker.Repositories;

namespace BugTracker.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repo;
    public UsuarioService(IUsuarioRepository repo) => _repo = repo;

    public async Task<IEnumerable<UsuarioResponseDTO>> GetAllAsync()
    {
        var usuarios = await _repo.GetAllAsync();
        return usuarios.Select(u => new UsuarioResponseDTO
        {
            Id = u.Id, Nome = u.Nome, Email = u.Email, Perfil = u.Perfil, CriadoEm = u.CriadoEm
        });
    }

    public async Task<UsuarioResponseDTO?> GetByIdAsync(int id)
    {
        var u = await _repo.GetByIdAsync(id);
        if (u == null) return null;
        return new UsuarioResponseDTO { Id = u.Id, Nome = u.Nome, Email = u.Email, Perfil = u.Perfil, CriadoEm = u.CriadoEm };
    }

    public async Task<UsuarioResponseDTO?> UpdateAsync(int id, UsuarioUpdateDTO dto)
    {
        var u = await _repo.GetByIdAsync(id);
        if (u == null) return null;
        var perfisValidos = new[] { "Admin", "Dev" };
        if (dto.Nome != null) u.Nome = dto.Nome;
        if (dto.Email != null) u.Email = dto.Email;
        if (dto.Perfil != null && perfisValidos.Contains(dto.Perfil)) u.Perfil = dto.Perfil;
        await _repo.UpdateAsync(u);
        return new UsuarioResponseDTO { Id = u.Id, Nome = u.Nome, Email = u.Email, Perfil = u.Perfil, CriadoEm = u.CriadoEm };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var u = await _repo.GetByIdAsync(id);
        if (u == null) return false;
        await _repo.DeleteAsync(u);
        return true;
    }
}
