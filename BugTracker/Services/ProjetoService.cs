using BugTracker.DTOs;
using BugTracker.Models;
using BugTracker.Repositories;

namespace BugTracker.Services;

public class ProjetoService : IProjetoService
{
    private readonly IProjetoRepository _repo;
    public ProjetoService(IProjetoRepository repo) => _repo = repo;

    public async Task<IEnumerable<ProjetoResponseDTO>> GetAllAsync()
    {
        var projetos = await _repo.GetAllAsync();
        return projetos.Select(p => ToDTO(p));
    }

    public async Task<ProjetoResponseDTO?> GetByIdAsync(int id)
    {
        var p = await _repo.GetByIdAsync(id);
        return p == null ? null : ToDTO(p);
    }

    public async Task<ProjetoResponseDTO> CreateAsync(ProjetoCreateDTO dto)
    {
        var projeto = new Projeto
        {
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            MotorJogo = dto.MotorJogo,
            PlataformasAlvo = dto.PlataformasAlvo,
            VersaoAtual = dto.VersaoAtual
        };
        await _repo.CreateAsync(projeto);
        return ToDTO(projeto);
    }

    public async Task<ProjetoResponseDTO?> UpdateAsync(int id, ProjetoUpdateDTO dto)
    {
        var p = await _repo.GetByIdAsync(id);
        if (p == null) return null;
        if (dto.Nome != null) p.Nome = dto.Nome;
        if (dto.Descricao != null) p.Descricao = dto.Descricao;
        if (dto.MotorJogo != null) p.MotorJogo = dto.MotorJogo;
        if (dto.PlataformasAlvo != null) p.PlataformasAlvo = dto.PlataformasAlvo;
        if (dto.VersaoAtual != null) p.VersaoAtual = dto.VersaoAtual;
        await _repo.UpdateAsync(p);
        return ToDTO(p);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var p = await _repo.GetByIdAsync(id);
        if (p == null) return false;
        await _repo.DeleteAsync(p);
        return true;
    }

    private static ProjetoResponseDTO ToDTO(Projeto p) => new()
    {
        Id = p.Id,
        Nome = p.Nome,
        Descricao = p.Descricao,
        MotorJogo = p.MotorJogo,
        PlataformasAlvo = p.PlataformasAlvo,
        VersaoAtual = p.VersaoAtual,
        CriadoEm = p.CriadoEm,
        TotalBugs = p.Bugs.Count
    };
}
