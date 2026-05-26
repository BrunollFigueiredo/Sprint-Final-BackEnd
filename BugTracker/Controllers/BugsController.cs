using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BugTracker.DTOs;
using BugTracker.Services;

namespace BugTracker.Controllers;

[ApiController]
[Route("api/bugs")]
[Authorize]
public class BugsController : ControllerBase
{
    private readonly IBugService _service;
    public BugsController(IBugService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("projeto/{projetoId}")]
    public async Task<IActionResult> GetByProjeto(int projetoId) =>
        Ok(await _service.GetByProjetoIdAsync(projetoId));

    [HttpPost]
    [Authorize(Roles = "Admin,Dev")]
    public async Task<IActionResult> Create([FromBody] BugCreateDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        try
        {
            var result = await _service.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Dev")]
    public async Task<IActionResult> Update(int id, [FromBody] BugUpdateDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isAdmin = User.IsInRole("Admin");
        try
        {
            var result = await _service.UpdateAsync(id, dto, userId, isAdmin);
            return result == null ? NotFound() : Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
