namespace BugTracker.Models;

public class BugTag
{
    public int BugId { get; set; }
    public Bug Bug { get; set; } = null!;
    public int TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}
