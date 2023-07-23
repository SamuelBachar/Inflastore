using InflaStoreWebAPI.Models.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace InflaStoreWebAPI.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer("Server=.\\INFLASTORE;Database=userdb;Trusted_Connection=true;TrustServerCertificate=True;");
    }

    public DbSet<User> Users => Set<User>();
}
