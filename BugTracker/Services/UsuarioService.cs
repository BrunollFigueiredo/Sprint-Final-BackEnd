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
        return usuarios.Select(ToDTO);
    }

    public async Task<UsuarioResponseDTO?> GetByIdAsync(int id)
    {
        var u = await _repo.GetByIdAsync(id);
        return u == null ? null : ToDTO(u);
    }

    public async Task<UsuarioResponseDTO?> UpdateAsync(int id, UsuarioUpdateDTO dto)
    {
        var u = await _repo.GetByIdAsync(id);
        if (u == null) return null;

        var perfisValidos = new[] { "Admin", "Dev" };
        var cargosValidos = new[] { "Lider", "Programador", "Designer", "Artista", "SoundDesigner", "QA" };

        if (dto.Nome != null) u.Nome = dto.Nome;
        if (dto.Email != null) u.Email = dto.Email;
        if (dto.Perfil != null && perfisValidos.Contains(dto.Perfil)) u.Perfil = dto.Perfil;
        if (dto.Cargo != null && cargosValidos.Contains(dto.Cargo)) u.Cargo = dto.Cargo;

        await _repo.UpdateAsync(u);
        return ToDTO(u);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var u = await _repo.GetByIdAsync(id);
        if (u == null) return false;
        await _repo.DeleteAsync(u);
        return true;
    }

    private static UsuarioResponseDTO ToDTO(BugTracker.Models.Usuario u) => new()
    {
        Id = u.Id,
        Nome = u.Nome,
        Email = u.Email,
        Perfil = u.Perfil,
        Cargo = u.Cargo,
        CriadoEm = u.CriadoEm
    };
}
