using System.ComponentModel.DataAnnotations;

namespace BugTracker.DTOs;

public class BugCreateDTO
{
    public List<int>? TagIds { get; set; }
    public List<PassoReproducaoDTO>? PassosReproducao { get; set; }
    [Required, MaxLength(200)]
    public string Titulo { get; set; } = string.Empty;
    [MaxLength(2000)]
    public string? Descricao { get; set; }
    [Required]
    public string Severidade { get; set; } = "Baixa";
    [Required]
    public int ProjetoId { get; set; }
    public int? AtribuidoParaId { get; set; }
    [MaxLength(50)]
    public string? TipoBug { get; set; }
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
}

public class BugUpdateDTO
{
    public List<int>? TagIds { get; set; }
    public List<PassoReproducaoDTO>? PassosReproducao { get; set; }
    [MaxLength(200)]
    public string? Titulo { get; set; }
    [MaxLength(2000)]
    public string? Descricao { get; set; }
    public string? Severidade { get; set; }
    public string? Status { get; set; }
    public int? AtribuidoParaId { get; set; }
    [MaxLength(50)]
    public string? TipoBug { get; set; }
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
    public bool? BloqueiaLancamento { get; set; }
    [MaxLength(1000)]
    public string? ResultadoEsperado { get; set; }
    [MaxLength(1000)]
    public string? ResultadoObtido { get; set; }
    [MaxLength(500)]
    public string? DetalhesAmbiente { get; set; }
}

public class BugResponseDTO
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string Severidade { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? TipoBug { get; set; }
    public string? Plataforma { get; set; }
    public string? VersaoJogo { get; set; }
    public string? NumeroBuild { get; set; }
    public string? Milestone { get; set; }
    public string? Cena { get; set; }
    public string? FrequenciaReproducao { get; set; }
    public bool BloqueiaLancamento { get; set; }
    public string? ResultadoEsperado { get; set; }
    public string? ResultadoObtido { get; set; }
    public string? DetalhesAmbiente { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
    public int ProjetoId { get; set; }
    public string ProjetoNome { get; set; } = string.Empty;
    public int ReportadoPorId { get; set; }
    public string ReportadoPorNome { get; set; } = string.Empty;
    public int? AtribuidoParaId { get; set; }
    public string? AtribuidoParaNome { get; set; }
    public string? AtribuidoParaCargo { get; set; }
    public string ReportadoPorCargo { get; set; } = string.Empty;
    public List<TagResponseDTO> Tags { get; set; } = new();
    public List<PassoReproducaoDTO>? PassosReproducao { get; set; }
    public List<BugMediaDTO> Media { get; set; } = new();
    public int DiasAberto { get; set; }
    public int TotalComentarios { get; set; }
}
