using System.ComponentModel.DataAnnotations;

namespace BugTracker.DTOs;

public class ComentarioCreateDTO
{
    [Required, MaxLength(1000)]
    public string Texto { get; set; } = string.Empty;
}

public class ComentarioResponseDTO
{
    public int Id { get; set; }
    public string Texto { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
    public int UsuarioId { get; set; }
    public string UsuarioNome { get; set; } = string.Empty;
    public string UsuarioPerfil { get; set; } = string.Empty;
}
