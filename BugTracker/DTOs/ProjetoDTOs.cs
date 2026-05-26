using System.ComponentModel.DataAnnotations;

namespace BugTracker.DTOs;

public class ProjetoCreateDTO
{
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
}

public class ProjetoUpdateDTO
{
    [MaxLength(150)]
    public string? Nome { get; set; }
    [MaxLength(500)]
    public string? Descricao { get; set; }
    [MaxLength(50)]
    public string? MotorJogo { get; set; }
    [MaxLength(300)]
    public string? PlataformasAlvo { get; set; }
    [MaxLength(30)]
    public string? VersaoAtual { get; set; }
}

public class ProjetoResponseDTO
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string? MotorJogo { get; set; }
    public string? PlataformasAlvo { get; set; }
    public string? VersaoAtual { get; set; }
    public DateTime CriadoEm { get; set; }
    public int TotalBugs { get; set; }
}
