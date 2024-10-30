using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext // DbContext Database bağlantısı
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    public DbSet<Survey> Surveys { get; set; }
    public DbSet<SurveyResponse> SurveyResponses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Survey>().ToTable("Surveys");
        modelBuilder.Entity<SurveyResponse>().ToTable("SurveyResponses"); // Düzeltildi
    }
}
