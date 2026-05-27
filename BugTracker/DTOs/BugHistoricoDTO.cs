namespace BugTracker.DTOs;

public class BugHistoricoDTO
{
    public int Id { get; set; }
    public string StatusAnterior { get; set; } = string.Empty;
    public string StatusNovo { get; set; } = string.Empty;
    public string UsuarioNome { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
}
