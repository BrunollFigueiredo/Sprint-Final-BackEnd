using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using BugTracker.DTOs;
using BugTracker.Models;
using BugTracker.Repositories;

namespace BugTracker.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepo;
    private readonly IConfiguration _config;

    public AuthService(IUsuarioRepository usuarioRepo, IConfiguration config)
    {
        _usuarioRepo = usuarioRepo;
        _config = config;
    }

    public async Task<AuthResponseDTO?> RegisterAsync(RegisterDTO dto)
    {
        var existing = await _usuarioRepo.GetByEmailAsync(dto.Email);
        if (existing != null) return null;

        if (!dto.AceitouTermos) return null;

        var usuario = new Usuario
        {
            Nome = dto.Nome,
            Email = dto.Email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
            Perfil = "Dev",
            Cargo = "Programador",
            AceitouTermos = true,
            AceitouTermosEm = DateTime.UtcNow
        };

        await _usuarioRepo.CreateAsync(usuario);
        return new AuthResponseDTO
        {
            Id = usuario.Id,
            Token = GerarToken(usuario),
            Nome = usuario.Nome,
            Email = usuario.Email,
            Perfil = usuario.Perfil,
            Cargo = usuario.Cargo
        };
    }

    public async Task<AuthResponseDTO?> LoginAsync(LoginDTO dto)
    {
        var usuario = await _usuarioRepo.GetByEmailAsync(dto.Email);
        if (usuario == null) return null;
        if (!BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash)) return null;

        return new AuthResponseDTO
        {
            Id = usuario.Id,
            Token = GerarToken(usuario),
            Nome = usuario.Nome,
            Email = usuario.Email,
            Perfil = usuario.Perfil,
            Cargo = usuario.Cargo
        };
    }

    private string GerarToken(Usuario usuario)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Role, usuario.Perfil)
        };
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
