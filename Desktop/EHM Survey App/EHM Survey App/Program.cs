using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Linq;
using System.IO; // Log dosyaları için gerekli
using static LogCleaner; // Log temizleme işlemi için

var builder = WebApplication.CreateBuilder(args);

// Serilog yapılandırmasını appsettings.json'dan okuma
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add custom services
builder.Services.AddScoped<SmsService>();
builder.Services.AddScoped<QRCodeGeneratorService>(provider => 
    new QRCodeGeneratorService("http://localhost:5139/api/survey"));

// Configure CORS to allow all origins, methods, and headers
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Log temizleme işlevini çağırma
LogCleaner.CleanUpOldLogs("Logs", 3);

var app = builder.Build();

// Generate QR codes for each store on startup
using (var scope = app.Services.CreateScope())
{
    var qrCodeService = scope.ServiceProvider.GetRequiredService<QRCodeGeneratorService>();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var storeCodes = dbContext.Stores.Select(s => s.StoreCode).ToList();

    foreach (var StoreCode in storeCodes)
    {
        try
        {
            qrCodeService.GenerateQRCode(StoreCode);
            Console.WriteLine($"{StoreCode} için QR kod başarıyla oluşturuldu.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"QR kod oluşturma sırasında hata: {ex.Message}");
        }
    }
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAllOrigins"); // Apply the CORS policy
app.UseAuthorization();

// Allow external access
app.Urls.Add("http://0.0.0.0:5139");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
