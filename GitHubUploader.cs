using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

public class GitHubUploader
{
    private readonly string _owner;
    private readonly string _repo;
    private readonly string _token;
    private readonly HttpClient _httpClient;

    public GitHubUploader(string owner, string repo, string token)
    {
        _owner = owner;
        _repo = repo;
        _token = token;
        _httpClient = new HttpClient();

        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("MakerSpaceApp");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", _token);
    }

    public async Task<bool> UploadOrUpdateFileAsync(string path, string content, string commitMessage)
    {
        var baseUrl = $"https://api.github.com/repos/{_owner}/{_repo}/contents/{path}";

        // Prüfe, ob Datei existiert, um sha für Update zu bekommen
        var getResponse = await _httpClient.GetAsync(baseUrl);
        string? sha = null;
        if (getResponse.IsSuccessStatusCode)
        {
            var json = await getResponse.Content.ReadFromJsonAsync<JsonDocument>();
            if (json != null && json.RootElement.TryGetProperty("sha", out var shaElement))
            {
                sha = shaElement.GetString();
            }
        }

        // GitHub erwartet Base64-kodierten Inhalt
        var base64Content = Convert.ToBase64String(Encoding.UTF8.GetBytes(content));

        var payload = new
        {
            message = commitMessage,
            content = base64Content,
            sha = sha
        };

        var payloadJson = JsonSerializer.Serialize(payload);

        var requestContent = new StringContent(payloadJson, Encoding.UTF8, "application/json");

        var putResponse = await _httpClient.PutAsync(baseUrl, requestContent);

        return putResponse.IsSuccessStatusCode;
    }
}
