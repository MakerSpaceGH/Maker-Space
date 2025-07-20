using Microsoft.Extensions.Options;
using System.IO;
using System.Threading.Tasks;

public class BackupService
{
    private readonly GitHubUploader _uploader;

    public BackupService(IOptions<GitHubSettings> githubSettings)
    {
        var settings = githubSettings.Value;
        _uploader = new GitHubUploader(settings.Owner, settings.Repo, settings.Token);
    }

    public async Task<bool> BackupFileAsync(string localFilePath, string repoFilePath)
    {
        if (!File.Exists(localFilePath))
            return false;

        var content = await File.ReadAllTextAsync(localFilePath);
        return await _uploader.UploadOrUpdateFileAsync(repoFilePath, content, $"Backup {repoFilePath} via API");
    }
}
