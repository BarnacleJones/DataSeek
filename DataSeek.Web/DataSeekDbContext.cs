using DataSeek.Web.DataModels;
using Microsoft.EntityFrameworkCore;

namespace DataSeek.Web;

public class DataSeekDbContext : DbContext
{
    public DataSeekDbContext(DbContextOptions<DataSeekDbContext> options) : base(options)
    {
    }

    
    public DbSet<UploadLine> UploadLines { get; set; }
    public DbSet<UploadFile> UploadFiles { get; set; }
    public DbSet<DownloadFile> DownloadFiles { get; set; }
}