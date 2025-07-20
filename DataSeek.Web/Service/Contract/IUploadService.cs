namespace DataSeek.Web.Service.Contract;

public interface IUploadService
{
    Task ProcessUploadAsync(IFormFile? file);
    Task ProcessDownloadAsync(IFormFile? file);
}