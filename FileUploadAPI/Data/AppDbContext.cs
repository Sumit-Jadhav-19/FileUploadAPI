using FileUploadAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FileUploadAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<ImageModel> Images { get; set; }
    }
}
