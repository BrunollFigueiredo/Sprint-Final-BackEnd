using System.ComponentModel.DataAnnotations;

namespace BugTracker.DTOs;

public class UsuarioResponseDTO
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Perfil { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
}

public class UsuarioUpdateDTO
{
    [MaxLength(100)]
    public string? Nome { get; set; }
    [MaxLength(150), EmailAddress]
    public string? Email { get; set; }
    public string? Perfil { get; set; }
}
