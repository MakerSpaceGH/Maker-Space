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
        // appsettings.json laden
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var section = config.GetSection("GitHub");
        _owner = section["Owner"] ?? throw new Exception("Owner fehlt in appsettings.json");
        _repo = section["Repo"] ?? throw new Exception("Repo fehlt in appsettings.json");
        _token = section["Token"] ?? throw new Exception("Token fehlt in appsettings.json");
    }

    public async Task<bool> UploadOrUpdateFileAsync(string path, string content, string commitMessage)
    {
        var baseUrl = $"https://api.github.com/repos/{_owner}/{_repo}/contents/{path}";
        using var http = new HttpClient();

        http.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("MakerSpaceUploader", "1.0"));
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", _token);

        string sha = null;

        // Prüfen, ob Datei existiert
        var response = await http.GetAsync(baseUrl);
        if (response.IsSuccessStatusCode)
        {
            var existingFileJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(existingFileJson);
            sha = doc.RootElement.GetProperty("sha").GetString();
        }

        // Payload vorbereiten
        var payload = new
        {
            message = commitMessage,
            content = Convert.ToBase64String(Encoding.UTF8.GetBytes(content)),
            sha = sha
        };

        var jsonPayload = JsonSerializer.Serialize(payload);
        var putContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        // Upload oder Update
        var putResponse = await http.PutAsync(baseUrl, putContent);
        return putResponse.IsSuccessStatusCode;
    }
}
