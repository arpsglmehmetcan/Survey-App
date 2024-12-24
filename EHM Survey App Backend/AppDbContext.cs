using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using EHM_Survey_App_Backend.Models;
using System.Linq;
using System.Collections.Generic;

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
        // Tablo eşlemeleri
        modelBuilder.Entity<Survey>().ToTable("Surveys");
        modelBuilder.Entity<SurveyResponse>().ToTable("SurveyResponses");
        modelBuilder.Entity<Store>().ToTable("Stores");
        modelBuilder.Entity<UserRole>().ToTable("UserRole");

        // ValueComparer tanımı
        var intListComparer = new ValueComparer<List<int>>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList());

        // UserRole için StoreIds alanını dönüştürme (List<int> -> string ve tersi)
        modelBuilder.Entity<UserRole>()
            .Property(e => e.StoreIds)
            .HasConversion(
                v => string.Join(',', v), // StoreIds listesini bir string olarak kaydet
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList() // String'i listeye çevir
            )
            .Metadata.SetValueComparer(intListComparer);
    }
}