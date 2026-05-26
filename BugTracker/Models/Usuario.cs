using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models;

public class Usuario
{
    public int Id { get; set; }
    [Required, MaxLength(100)]
    public string Nome { get; set; } = string.Empty;
    [Required, MaxLength(150)]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string SenhaHash { get; set; } = string.Empty;
    [Required]
    public string Perfil { get; set; } = "Dev";
    public bool AceitouTermos { get; set; } = false;
    public DateTime? AceitouTermosEm { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public ICollection<Bug> BugsReportados { get; set; } = new List<Bug>();
    public ICollection<Bug> BugsAtribuidos { get; set; } = new List<Bug>();
}
