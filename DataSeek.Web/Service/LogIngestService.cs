using DataSeek.Web.DataModels;
using DataSeek.Web.Service.Contract;
using Microsoft.Extensions.Options;

namespace DataSeek.Web.Service;

public class LogIngestService : BackgroundService
{
    private readonly ILogger<LogIngestService> _logger;
    private readonly IServiceProvider _services;
    private readonly IOptions<LogIngestOptions> _options;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(1); // adjust as needed

    public LogIngestService(
        ILogger<LogIngestService> logger,
        IServiceProvider services,
        IOptions<LogIngestOptions> options)
    {
        _logger = logger;
        _services = services;
        _options = options;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessLogsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing logs");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task ProcessLogsAsync()
    {
        var dir = _options.Value.LogDirectory;
        var processedDir = Path.Combine(dir, "processed");

        // Ensure the processed folder exists
        Directory.CreateDirectory(processedDir);

        var uploadPattern = _options.Value.UploadPattern;
        var downloadPattern = _options.Value.DownloadPattern;

        var uploadFiles = Directory.GetFiles(dir, uploadPattern);
        var downloadFiles = Directory.GetFiles(dir, downloadPattern);

        using var scope = _services.CreateScope();
        var uploadService = scope.ServiceProvider.GetRequiredService<IUploadService>();

        foreach (var filePath in uploadFiles)
        {
            try
            {
                await using var fs = File.OpenRead(filePath);
                var formFile = new FormFile(fs, 0, fs.Length, "file", Path.GetFileName(filePath));
                await uploadService.ProcessUploadAsync(formFile);
                MoveToProcessed(filePath, processedDir);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing upload file {File}", filePath);
            }
        }

        foreach (var filePath in downloadFiles)
        {
            try
            {
                await using var fs = File.OpenRead(filePath);
                var formFile = new FormFile(fs, 0, fs.Length, "file", Path.GetFileName(filePath));
                await uploadService.ProcessDownloadAsync(formFile);
                MoveToProcessed(filePath, processedDir);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing download file {File}", filePath);
            }
        }
    }
    private void MoveToProcessed(string originalPath, string processedDir)
    {
        var fileName = Path.GetFileName(originalPath);
        var destinationPath = Path.Combine(processedDir, fileName);

        // Avoid overwriting if somehow a file already exists
        if (File.Exists(destinationPath))
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var newName = $"{Path.GetFileNameWithoutExtension(fileName)}_{timestamp}{Path.GetExtension(fileName)}";
            destinationPath = Path.Combine(processedDir, newName);
        }

        File.Move(originalPath, destinationPath);
    }

}
