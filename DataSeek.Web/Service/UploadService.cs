using System.Globalization;
using System.Text.RegularExpressions;
using DataSeek.Web.DataModels;
using DataSeek.Web.Service.Contract;

namespace DataSeek.Web.Service;

public class UploadService(DataSeekDbContext context) : IUploadService
{
    public async Task ProcessUploadAsync(IFormFile? file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty.");

        var uploadFile = new UploadFile
        {
            UploadFileName = file.FileName,
            UploadLines = new List<UploadLine>()
        };

        using var stream = new StreamReader(file.OpenReadStream());
        string? line;
        while ((line = await stream.ReadLineAsync()) != null)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            // Sample line:
            // 03/22/25 01:09:36 Upload finished: user testinUser, IP address ('99.245.99.142', 28771), file MusicBrainzMusic\Gnod\...\01 Bits.mp3
            var match = Regex.Match(line, @"^(?<timestamp>\d{2}/\d{2}/\d{2} \d{2}:\d{2}:\d{2}) Upload (started|finished): user (?<user>.*?), IP address \('(?<ip>[\d\.]+)', \d+\), file (?<filepath>.+)$");

            if (!match.Success)
                continue;

            var timestamp = match.Groups["timestamp"].Value;
            var user = match.Groups["user"].Value;
            var ip = match.Groups["ip"].Value;
            var filepath = match.Groups["filepath"].Value;

            var parsed = DateTime.TryParseExact(
                timestamp,
                "MM/dd/yy HH:mm:ss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsedDate
            );

            var uploadLine = new UploadLine
            {
                UploadDate = parsed ? DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc) : DateTime.UtcNow,
                User = user,
                IpAddress = ip,
                FileName = filepath,
                UploadFile = uploadFile
            };

            uploadFile.UploadLines.Add(uploadLine);
        }

        context.UploadFiles.Add(uploadFile);
        await context.SaveChangesAsync();
    }

    public async Task ProcessDownloadAsync(IFormFile? file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty.");

        var downloadFile = new DownloadFile
        {
            DownloadFileName = file.FileName,
            DownloadLines = new List<DownloadLine>()
        };

        using var stream = new StreamReader(file.OpenReadStream());
        string? line;
        while ((line = await stream.ReadLineAsync()) != null)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var match = Regex.Match(line, @"^(?<timestamp>\d{2}/\d{2}/\d{2} \d{2}:\d{2}:\d{2}) Download (started|finished): user (?<user>.*?), file (?<filepath>.+)$");

            if (!match.Success)
                continue;

            var timestamp = match.Groups["timestamp"].Value;
            var user = match.Groups["user"].Value;
            var filepath = match.Groups["filepath"].Value;

            var parsed = DateTime.TryParseExact(
                timestamp,
                "MM/dd/yy HH:mm:ss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out var parsedDate
            );

            var downloadLine = new DownloadLine
            {
                DownloadDate = parsed ? parsedDate : DateTime.UtcNow,
                User = user,
                FileName = filepath,
                DownloadFile = downloadFile
            };

            downloadFile.DownloadLines.Add(downloadLine);
        }

        context.DownloadFiles.Add(downloadFile);
        await context.SaveChangesAsync();
    }

}