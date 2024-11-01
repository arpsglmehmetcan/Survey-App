using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Survey> Surveys { get; set; }
    public DbSet<SurveyResponse> SurveyResponses { get; set; }
    public DbSet<Store> Stores { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Survey>().ToTable("Surveys");
        modelBuilder.Entity<SurveyResponse>().ToTable("SurveyResponses");
        modelBuilder.Entity<Store>().ToTable("Stores");

        // Başlangıç verileri eklendi
        modelBuilder.Entity<Store>().HasData(
            new Store { StoreId = 1, StoreCode = "STORE123", StoreName = "Mağaza 1" },
            new Store { StoreId = 2, StoreCode = "STORE456", StoreName = "Mağaza 2" },
            new Store { StoreId = 3, StoreCode = "STORE789", StoreName = "Mağaza 3" }
        );
    }
}
