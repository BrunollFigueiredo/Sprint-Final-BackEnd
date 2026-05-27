using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models;

public class BugMedia
{
    public int Id { get; set; }
    public int BugId { get; set; }
    public Bug Bug { get; set; } = null!;
    [MaxLength(255)]
    public string NomeOriginal { get; set; } = string.Empty;
    [MaxLength(500)]
    public string StoragePath { get; set; } = string.Empty;
    [MaxLength(500)]
    public string Url { get; set; } = string.Empty;
    [MaxLength(100)]
    public string ContentType { get; set; } = string.Empty;
    public long TamanhoBytes { get; set; }
    public int UploadedByUserId { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
