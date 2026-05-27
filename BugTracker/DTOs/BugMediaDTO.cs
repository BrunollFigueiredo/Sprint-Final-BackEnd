namespace BugTracker.DTOs;

public class BugMediaDTO
{
    public int Id { get; set; }
    public string NomeOriginal { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long TamanhoBytes { get; set; }
    public int UploadedByUserId { get; set; }
    public DateTime CriadoEm { get; set; }
}
