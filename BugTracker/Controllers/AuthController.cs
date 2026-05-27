using Microsoft.AspNetCore.Mvc;
using BugTracker.DTOs;
using BugTracker.Services;

namespace BugTracker.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
    {
        if (!ModelState.IsValid)
        {
            var erro = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage ?? "Dados inválidos.";
            return BadRequest(new { mensagem = erro });
        }
        if (!dto.AceitouTermos)
            return BadRequest(new { mensagem = "Você deve aceitar os Termos de Uso e a Política de Privacidade para criar uma conta." });
        var result = await _authService.RegisterAsync(dto);
        if (result == null) return BadRequest(new { mensagem = "Email já cadastrado." });
        return CreatedAtAction(nameof(Register), result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        if (!ModelState.IsValid)
        {
            var erro = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage ?? "Dados inválidos.";
            return BadRequest(new { mensagem = erro });
        }
        var result = await _authService.LoginAsync(dto);
        if (result == null) return Unauthorized(new { mensagem = "Email ou senha inválidos." });
        return Ok(result);
    }
}
