using System.IO;
using System.Threading.Tasks;

public class BackupService
{
    private readonly GitHubUploader _uploader;

    public BackupService()
    {
        _uploader = new GitHubUploader();
    }

    public async Task<bool> BackupFileAsync(string localFilePath, string repoFilePath)
    {
        if (!File.Exists(localFilePath))
            return false;

        var content = File.ReadAllText(localFilePath);
        return await _uploader.UploadOrUpdateFileAsync(repoFilePath, content, $"Backup {repoFilePath} via API");
    }
}
