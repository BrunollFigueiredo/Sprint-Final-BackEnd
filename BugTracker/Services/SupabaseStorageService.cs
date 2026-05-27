using System.Net.Http.Headers;

namespace BugTracker.Services;

public class SupabaseStorageService : ISupabaseStorageService
{
    private readonly HttpClient _http;
    private readonly string _projectUrl;
    private readonly string _serviceKey;
    private const string Bucket = "bug-media";

    public SupabaseStorageService(IConfiguration config, IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("supabase");
        _projectUrl = config["Supabase:Url"]!.TrimEnd('/');
        _serviceKey = config["Supabase:ServiceKey"]!;
    }

    public async Task<(string StoragePath, string PublicUrl)> UploadAsync(IFormFile file, int bugId)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid()}{ext}";
        var storagePath = $"bugs/{bugId}/{fileName}";

        using var stream = file.OpenReadStream();
        using var content = new StreamContent(stream);
        content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

        var request = new HttpRequestMessage(HttpMethod.Post,
            $"{_projectUrl}/storage/v1/object/{Bucket}/{storagePath}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _serviceKey);
        request.Headers.Add("x-upsert", "true");
        request.Content = content;

        var response = await _http.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new Exception($"Supabase upload falhou ({response.StatusCode}): {body}");
        }

        var publicUrl = $"{_projectUrl}/storage/v1/object/public/{Bucket}/{storagePath}";
        return (storagePath, publicUrl);
    }

    public async Task DeleteAsync(string storagePath)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete,
            $"{_projectUrl}/storage/v1/object/{Bucket}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _serviceKey);
        request.Content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(new { prefixes = new[] { storagePath } }),
            System.Text.Encoding.UTF8,
            "application/json");
        await _http.SendAsync(request);
    }
}
