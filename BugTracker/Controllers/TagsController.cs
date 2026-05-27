using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BugTracker.DTOs;
using BugTracker.Services;

namespace BugTracker.Controllers;

[ApiController]
[Route("api/tags")]
[Authorize]
public class TagsController : ControllerBase
{
    private readonly ITagService _service;
    public TagsController(ITagService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tags = await _service.GetAllAsync();
        return Ok(tags);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] TagCreateDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var tag = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetAll), new { id = tag.Id }, tag);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound(new { mensagem = "Tag não encontrada." });
        return NoContent();
    }
}
