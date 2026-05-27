using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BugTracker.DTOs;
using BugTracker.Repositories;

namespace BugTracker.Controllers;

[ApiController]
[Route("api/bugs/{bugId}/historico")]
[Authorize]
public class BugHistoricoController : ControllerBase
{
    private readonly IBugHistoricoRepository _repo;
    public BugHistoricoController(IBugHistoricoRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> GetHistorico(int bugId)
    {
        var historico = await _repo.GetByBugIdAsync(bugId);
        var result = historico.Select(h => new BugHistoricoDTO
        {
            Id = h.Id,
            StatusAnterior = h.StatusAnterior,
            StatusNovo = h.StatusNovo,
            UsuarioNome = h.Usuario?.Nome ?? "",
            CriadoEm = h.CriadoEm
        });
        return Ok(result);
    }
}
