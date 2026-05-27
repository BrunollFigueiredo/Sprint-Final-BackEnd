using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models;

public class Tag
{
    public int Id { get; set; }
    [Required, MaxLength(50)]
    public string Nome { get; set; } = string.Empty;
    [MaxLength(7)]
    public string Cor { get; set; } = "#6c757d"; // hex color
    [MaxLength(50)]
    public string Departamento { get; set; } = "Geral";
    public ICollection<BugTag> BugTags { get; set; } = new List<BugTag>();
}
