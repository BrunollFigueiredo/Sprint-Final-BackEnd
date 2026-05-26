using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BugTracker.Data;
using BugTracker.DTOs;

namespace BugTracker.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _db;
    public DashboardController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var bugs = await _db.Bugs.Include(b => b.Projeto).Include(b => b.AtribuidoPara).ToListAsync();
        var projetos = await _db.Projetos.ToListAsync();
        var usuarios = await _db.Usuarios.ToListAsync();

        var bugsPorProjeto = projetos
            .Select(p => new BugsPorProjetoDTO
            {
                Nome = p.Nome,
                Total = bugs.Count(b => b.ProjetoId == p.Id),
                Abertos = bugs.Count(b => b.ProjetoId == p.Id && b.Status == "Aberto")
            })
            .Where(x => x.Total > 0)
            .OrderByDescending(x => x.Total);

        var bugsPorSeveridade = new[] { "Critica", "Alta", "Media", "Baixa" }
            .Select(s => new BugsPorSeveridadeDTO
            {
                Severidade = s,
                Total = bugs.Count(b => b.Severidade == s)
            })
            .Where(x => x.Total > 0);

        var bugsPorPlataforma = bugs
            .Where(b => !string.IsNullOrEmpty(b.Plataforma))
            .GroupBy(b => b.Plataforma!)
            .Select(g => new BugsPorPlataformaDTO { Plataforma = g.Key, Total = g.Count() })
            .OrderByDescending(x => x.Total);

        var bugsPorTipo = bugs
            .Where(b => !string.IsNullOrEmpty(b.TipoBug))
            .GroupBy(b => b.TipoBug!)
            .Select(g => new BugsPorTipoDTO { Tipo = g.Key, Total = g.Count() })
            .OrderByDescending(x => x.Total);

        var bugsBloqueiadores = bugs
            .Where(b => b.BloqueiaLancamento && b.Status != "Resolvido" && b.Status != "Fechado")
            .OrderByDescending(b => b.Severidade == "Critica")
            .ThenByDescending(b => b.Severidade == "Alta")
            .Select(b => new BugBloqueiadorDTO
            {
                Id = b.Id,
                Titulo = b.Titulo,
                ProjetoNome = b.Projeto?.Nome ?? "",
                Severidade = b.Severidade,
                Status = b.Status,
                AtribuidoParaNome = b.AtribuidoPara?.Nome
            });

        return Ok(new DashboardDTO
        {
            TotalBugs = bugs.Count,
            BugsAbertos = bugs.Count(b => b.Status == "Aberto"),
            BugsEmAndamento = bugs.Count(b => b.Status == "EmAndamento"),
            BugsResolvidos = bugs.Count(b => b.Status == "Resolvido"),
            BugsFechados = bugs.Count(b => b.Status == "Fechado"),
            BugsCriticos = bugs.Count(b => b.Severidade == "Critica"),
            BugsBloqueiamLancamento = bugs.Count(b => b.BloqueiaLancamento && b.Status != "Resolvido" && b.Status != "Fechado"),
            TotalProjetos = projetos.Count,
            TotalUsuarios = usuarios.Count,
            BugsPorProjeto = bugsPorProjeto,
            BugsPorSeveridade = bugsPorSeveridade,
            BugsPorPlataforma = bugsPorPlataforma,
            BugsPorTipo = bugsPorTipo,
            BugsBloqueiadores = bugsBloqueiadores
        });
    }
}
