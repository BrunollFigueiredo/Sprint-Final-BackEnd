using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models;

public class Projeto
{
    public int Id { get; set; }
    [Required, MaxLength(150)]
    public string Nome { get; set; } = string.Empty;
    [MaxLength(500)]
    public string? Descricao { get; set; }
    [MaxLength(50)]
    public string? MotorJogo { get; set; }
    [MaxLength(300)]
    public string? PlataformasAlvo { get; set; }
    [MaxLength(30)]
    public string? VersaoAtual { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public ICollection<Bug> Bugs { get; set; } = new List<Bug>();
}
