using Microsoft.EntityFrameworkCore;
using EHM_Survey_App_Backend.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Survey> Surveys { get; set; }
    public DbSet<SurveyResponse> SurveyResponses { get; set; }
    public DbSet<Store> Stores { get; set; }
    public DbSet<UserRole> UserRole { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Survey>().ToTable("Surveys");
        modelBuilder.Entity<SurveyResponse>().ToTable("SurveyResponses");
        modelBuilder.Entity<Store>().ToTable("Stores");
        modelBuilder.Entity<UserRole>().ToTable("UserRole");
    }
}
