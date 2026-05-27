namespace BugTracker.Models;

public class BugHistorico
{
    public int Id { get; set; }
    public int BugId { get; set; }
    public Bug Bug { get; set; } = null!;
    public string StatusAnterior { get; set; } = string.Empty;
    public string StatusNovo { get; set; } = string.Empty;
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
