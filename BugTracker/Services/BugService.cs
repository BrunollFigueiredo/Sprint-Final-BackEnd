using System.Text.Json;
using BugTracker.DTOs;
using BugTracker.Models;
using BugTracker.Repositories;

namespace BugTracker.Services;

public class BugService : IBugService
{
    private readonly IBugRepository _repo;
    private readonly IProjetoRepository _projetoRepo;
    private readonly IUsuarioRepository _usuarioRepo;
    private readonly ITagRepository _tagRepo;
    private readonly IBugHistoricoRepository _historicoRepo;
    private readonly IComentarioRepository _comentarioRepo;

    public BugService(
        IBugRepository repo,
        IProjetoRepository projetoRepo,
        IUsuarioRepository usuarioRepo,
        ITagRepository tagRepo,
        IBugHistoricoRepository historicoRepo,
        IComentarioRepository comentarioRepo)
    {
        _repo = repo;
        _projetoRepo = projetoRepo;
        _usuarioRepo = usuarioRepo;
        _tagRepo = tagRepo;
        _historicoRepo = historicoRepo;
        _comentarioRepo = comentarioRepo;
    }

    public async Task<IEnumerable<BugResponseDTO>> GetAllAsync()
    {
        var bugs = await _repo.GetAllAsync();
        return bugs.Select(ToDTO);
    }

    public async Task<BugResponseDTO?> GetByIdAsync(int id)
    {
        var b = await _repo.GetByIdAsync(id);
        if (b == null) return null;
        var dto = ToDTO(b);
        var comentarios = await _comentarioRepo.GetByBugIdAsync(id);
        dto.TotalComentarios = comentarios.Count();
        return dto;
    }

    public async Task<IEnumerable<BugResponseDTO>> GetByProjetoIdAsync(int projetoId)
    {
        var bugs = await _repo.GetByProjetoIdAsync(projetoId);
        return bugs.Select(ToDTO);
    }

    public async Task<BugResponseDTO> CreateAsync(BugCreateDTO dto, int reportadoPorId)
    {
        if (await _projetoRepo.GetByIdAsync(dto.ProjetoId) == null)
            throw new ArgumentException($"Projeto com ID {dto.ProjetoId} não encontrado.");

        if (dto.AtribuidoParaId.HasValue && await _usuarioRepo.GetByIdAsync(dto.AtribuidoParaId.Value) == null)
            throw new ArgumentException($"Usuário com ID {dto.AtribuidoParaId.Value} não encontrado.");

        var severidadesValidas = new[] { "Baixa", "Media", "Alta", "Critica" };
        var severidade = severidadesValidas.Contains(dto.Severidade) ? dto.Severidade : "Baixa";

        var bug = new Bug
        {
            Titulo = dto.Titulo,
            Descricao = dto.Descricao,
            Severidade = severidade,
            Status = "Aberto",
            ProjetoId = dto.ProjetoId,
            ReportadoPorId = reportadoPorId,
            AtribuidoParaId = dto.AtribuidoParaId,
            TipoBug = dto.TipoBug,
            Plataforma = dto.Plataforma,
            VersaoJogo = dto.VersaoJogo,
            NumeroBuild = dto.NumeroBuild,
            Milestone = dto.Milestone,
            Cena = dto.Cena,
            FrequenciaReproducao = dto.FrequenciaReproducao,
            BloqueiaLancamento = dto.BloqueiaLancamento,
            ResultadoEsperado = dto.ResultadoEsperado,
            ResultadoObtido = dto.ResultadoObtido,
            DetalhesAmbiente = dto.DetalhesAmbiente,
            PassosReproducao = dto.PassosReproducao != null
                ? JsonSerializer.Serialize(dto.PassosReproducao)
                : null
        };

        if (dto.TagIds?.Count > 0)
        {
            var tags = await _tagRepo.GetByIdsAsync(dto.TagIds);
            bug.BugTags = tags.Select(t => new BugTag { Tag = t }).ToList();
        }

        await _repo.CreateAsync(bug);
        var created = await _repo.GetByIdAsync(bug.Id);
        return ToDTO(created!);
    }

    public async Task<BugResponseDTO?> UpdateAsync(int id, BugUpdateDTO dto, int userId, bool isAdmin)
    {
        var b = await _repo.GetByIdAsync(id);
        if (b == null) return null;

        if (!isAdmin && b.AtribuidoParaId != userId)
            throw new UnauthorizedAccessException("Dev só pode editar bugs atribuídos a ele.");

        if (dto.AtribuidoParaId.HasValue && await _usuarioRepo.GetByIdAsync(dto.AtribuidoParaId.Value) == null)
            throw new ArgumentException($"Usuário com ID {dto.AtribuidoParaId.Value} não encontrado.");

        var severidadesValidas = new[] { "Baixa", "Media", "Alta", "Critica" };
        var statusValidos = new[] { "Aberto", "EmAndamento", "Resolvido", "Fechado", "NaoReproduzivel", "NaoCorrigir", "Duplicado" };

        var statusAnterior = b.Status;

        if (dto.Titulo != null) b.Titulo = dto.Titulo;
        if (dto.Descricao != null) b.Descricao = dto.Descricao;
        if (dto.Severidade != null && severidadesValidas.Contains(dto.Severidade)) b.Severidade = dto.Severidade;
        if (dto.Status != null && statusValidos.Contains(dto.Status)) b.Status = dto.Status;
        b.AtribuidoParaId = dto.AtribuidoParaId;
        if (dto.TipoBug != null) b.TipoBug = dto.TipoBug;
        if (dto.Plataforma != null) b.Plataforma = dto.Plataforma;
        if (dto.VersaoJogo != null) b.VersaoJogo = dto.VersaoJogo;
        if (dto.NumeroBuild != null) b.NumeroBuild = dto.NumeroBuild;
        if (dto.Milestone != null) b.Milestone = dto.Milestone;
        if (dto.Cena != null) b.Cena = dto.Cena;
        if (dto.FrequenciaReproducao != null) b.FrequenciaReproducao = dto.FrequenciaReproducao;
        if (dto.BloqueiaLancamento.HasValue) b.BloqueiaLancamento = dto.BloqueiaLancamento.Value;
        if (dto.ResultadoEsperado != null) b.ResultadoEsperado = dto.ResultadoEsperado;
        if (dto.ResultadoObtido != null) b.ResultadoObtido = dto.ResultadoObtido;
        if (dto.DetalhesAmbiente != null) b.DetalhesAmbiente = dto.DetalhesAmbiente;
        if (dto.PassosReproducao != null)
            b.PassosReproducao = JsonSerializer.Serialize(dto.PassosReproducao);

        if (dto.TagIds != null)
        {
            b.BugTags.Clear();
            if (dto.TagIds.Count > 0)
            {
                var tags = await _tagRepo.GetByIdsAsync(dto.TagIds);
                foreach (var tag in tags)
                    b.BugTags.Add(new BugTag { BugId = b.Id, TagId = tag.Id, Tag = tag });
            }
        }

        b.AtualizadoEm = DateTime.UtcNow;
        await _repo.UpdateAsync(b);

        if (dto.Status != null && statusValidos.Contains(dto.Status) && b.Status != statusAnterior)
        {
            await _historicoRepo.CreateAsync(new BugHistorico
            {
                BugId = b.Id,
                StatusAnterior = statusAnterior,
                StatusNovo = b.Status,
                UsuarioId = userId
            });
        }

        var updated = await _repo.GetByIdAsync(id);
        return ToDTO(updated!);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var b = await _repo.GetByIdAsync(id);
        if (b == null) return false;
        await _repo.DeleteAsync(b);
        return true;
    }

    private static BugResponseDTO ToDTO(Bug b) => new()
    {
        Id = b.Id,
        Titulo = b.Titulo,
        Descricao = b.Descricao,
        Severidade = b.Severidade,
        Status = b.Status,
        TipoBug = b.TipoBug,
        Plataforma = b.Plataforma,
        VersaoJogo = b.VersaoJogo,
        NumeroBuild = b.NumeroBuild,
        Milestone = b.Milestone,
        Cena = b.Cena,
        FrequenciaReproducao = b.FrequenciaReproducao,
        BloqueiaLancamento = b.BloqueiaLancamento,
        ResultadoEsperado = b.ResultadoEsperado,
        ResultadoObtido = b.ResultadoObtido,
        DetalhesAmbiente = b.DetalhesAmbiente,
        CriadoEm = b.CriadoEm,
        AtualizadoEm = b.AtualizadoEm,
        ProjetoId = b.ProjetoId,
        ProjetoNome = b.Projeto?.Nome ?? "",
        ReportadoPorId = b.ReportadoPorId,
        ReportadoPorNome = b.ReportadoPor?.Nome ?? "",
        ReportadoPorCargo = b.ReportadoPor?.Cargo ?? "",
        AtribuidoParaId = b.AtribuidoParaId,
        AtribuidoParaNome = b.AtribuidoPara?.Nome,
        AtribuidoParaCargo = b.AtribuidoPara?.Cargo,
        Tags = b.BugTags.Select(bt => new TagResponseDTO
        {
            Id = bt.Tag.Id,
            Nome = bt.Tag.Nome,
            Cor = bt.Tag.Cor,
            Departamento = bt.Tag.Departamento
        }).ToList(),
        PassosReproducao = b.PassosReproducao != null
            ? JsonSerializer.Deserialize<List<PassoReproducaoDTO>>(b.PassosReproducao)
            : null,
        Media = b.BugMedias.Select(m => new BugMediaDTO
        {
            Id = m.Id,
            NomeOriginal = m.NomeOriginal,
            Url = m.Url,
            ContentType = m.ContentType,
            TamanhoBytes = m.TamanhoBytes,
            UploadedByUserId = m.UploadedByUserId,
            CriadoEm = m.CriadoEm
        }).ToList(),
        DiasAberto = (int)(DateTime.UtcNow - b.CriadoEm).TotalDays
    };
}
