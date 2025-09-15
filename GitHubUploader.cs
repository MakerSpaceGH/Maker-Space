using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class GitHubUploader
{
    private readonly string _owner;
    private readonly string _repo;
    private readonly string _token;

    public GitHubUploader()
    {
        // ✅ Erst Environment Variablen probieren (Render)
        _owner = Environment.GetEnvironmentVariable("GITHUB_OWNER");
        _repo = Environment.GetEnvironmentVariable("GITHUB_REPO");
        _token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");

        // ✅ Falls nicht vorhanden → auf appsettings.json zurückfallen (lokales Debugging)
        if (string.IsNullOrWhiteSpace(_owner) ||
            string.IsNullOrWhiteSpace(_repo) ||
            string.IsNullOrWhiteSpace(_token))
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) // optional gemacht!
                .Build();

            var section = config.GetSection("GitHub");
            _owner ??= section["Owner"];
            _repo ??= section["Repo"];
            _token ??= section["Token"];
        }

        if (string.IsNullOrWhiteSpace(_owner) ||
            string.IsNullOrWhiteSpace(_repo) ||
            string.IsNullOrWhiteSpace(_token))
        {
            throw new Exception("❌ Es ist ein Fehler aufgetreten, Versuche es später erneut!).");
        }
    }

    public async Task<bool> UploadOrUpdateFileAsync(string path, string content, string commitMessage)
    {
        var baseUrl = $"https://api.github.com/repos/{_owner}/{_repo}/contents/{path}";
        using var http = new HttpClient();

        http.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("MakerSpaceUploader", "1.0"));
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", _token);

        string sha = null;

        // 🔍 Prüfen, ob Datei existiert
        var response = await http.GetAsync(baseUrl);
        if (response.IsSuccessStatusCode)
        {
            var existingFileJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(existingFileJson);
            sha = doc.RootElement.GetProperty("sha").GetString();
        }

        // 📦 Payload vorbereiten
        var payload = new
        {
            message = commitMessage,
            content = Convert.ToBase64String(Encoding.UTF8.GetBytes(content)),
            sha = sha
        };

        var jsonPayload = JsonSerializer.Serialize(payload);
        var putContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        // 🚀 Upload oder Update
        var putResponse = await http.PutAsync(baseUrl, putContent);
        return putResponse.IsSuccessStatusCode;
    }
}
