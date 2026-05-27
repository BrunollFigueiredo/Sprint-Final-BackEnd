namespace BugTracker.Services;

public interface ISupabaseStorageService
{
    Task<(string StoragePath, string PublicUrl)> UploadAsync(IFormFile file, int bugId);
    Task DeleteAsync(string storagePath);
}
