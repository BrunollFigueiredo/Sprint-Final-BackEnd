using System.ComponentModel.DataAnnotations;

namespace BugTracker.DTOs;

public class RegisterDTO
{
    [Required, MaxLength(100)]
    public string Nome { get; set; } = string.Empty;
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required, MinLength(6)]
    public string Senha { get; set; } = string.Empty;
    public bool AceitouTermos { get; set; } = false;
}

public class LoginDTO
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Senha { get; set; } = string.Empty;
}

public class AuthResponseDTO
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Perfil { get; set; } = string.Empty;
}
