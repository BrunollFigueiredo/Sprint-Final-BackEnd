using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models;

public class Bug
{
    public int Id { get; set; }
    [Required, MaxLength(200)]
    public string Titulo { get; set; } = string.Empty;
    [MaxLength(2000)]
    public string? Descricao { get; set; }
    [Required]
    public string Severidade { get; set; } = "Baixa"; // Baixa, Media, Alta, Critica
    [Required]
    public string Status { get; set; } = "Aberto";
    [MaxLength(50)]
    public string? TipoBug { get; set; } // Crash, Visual, Áudio, Gameplay, Performance, UI/UX, Localização, Outro
    [MaxLength(50)]
    public string? Plataforma { get; set; }
    [MaxLength(30)]
    public string? VersaoJogo { get; set; }
    [MaxLength(30)]
    public string? NumeroBuild { get; set; }
    [MaxLength(100)]
    public string? Milestone { get; set; }
    [MaxLength(200)]
    public string? Cena { get; set; }
    [MaxLength(50)]
    public string? FrequenciaReproducao { get; set; }
    public bool BloqueiaLancamento { get; set; } = false;
    [MaxLength(1000)]
    public string? ResultadoEsperado { get; set; }
    [MaxLength(1000)]
    public string? ResultadoObtido { get; set; }
    [MaxLength(500)]
    public string? DetalhesAmbiente { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
    public int ProjetoId { get; set; }
    public Projeto Projeto { get; set; } = null!;
    public int ReportadoPorId { get; set; }
    public Usuario ReportadoPor { get; set; } = null!;
    public int? AtribuidoParaId { get; set; }
    public Usuario? AtribuidoPara { get; set; }
}
