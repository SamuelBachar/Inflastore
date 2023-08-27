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
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<ItemPrice> ItemsPrices => Set<ItemPrice>();
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<NavigationShopData> NavigationShopDatas => Set<NavigationShopData>();
}
