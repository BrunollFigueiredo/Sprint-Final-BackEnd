using System.ComponentModel.DataAnnotations;

namespace BugTracker.DTOs;

public class TagCreateDTO
{
    [Required, MaxLength(50)]
    public string Nome { get; set; } = string.Empty;
    [MaxLength(7)]
    public string Cor { get; set; } = "#6c757d";
    [MaxLength(50)]
    public string Departamento { get; set; } = "Geral";
}

public class TagResponseDTO
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cor { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
}
