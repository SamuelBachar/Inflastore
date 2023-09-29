using Microsoft.EntityFrameworkCore;

namespace InflaStoreWebAPI.Data;

public class DataContext : DbContext
{
    private readonly IConfiguration _config;
    public DataContext(DbContextOptions<DataContext> options, IConfiguration config) : base(options)
    {
        _config = config;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        //optionsBuilder.UseSqlServer("Server=.\\INFLASTORE;Database=userdb;Trusted_Connection=true;TrustServerCertificate=True;");
        optionsBuilder.UseSqlServer(_config.GetSection("ConnectionStrings:InflastoreDBConnection").Value);
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<ItemPrice> ItemsPrices => Set<ItemPrice>();
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<NavigationShopData> NavigationShopDatas => Set<NavigationShopData>();
    public DbSet<Region> Regions => Set<Region>();
    public DbSet<ClubCard> ClubCards => Set<ClubCard>();
    public DbSet<Category> Categories => Set<Category>();
}
