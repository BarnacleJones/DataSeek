using DataSeek.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DataSeek.Data;

public class DataSeekDbContext : DbContext
{
    public DataSeekDbContext(DbContextOptions<DataSeekDbContext> options)
    {
    }
    public DbSet<UploadLine> UploadLines { get; set; }
    public DbSet<UploadFile> UploadFiles { get; set; }
}