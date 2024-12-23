using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Linq;
using System.IO; // Log dosyaları için gerekli
using AutoMapper;
using EHM_Survey_App_Backend;

var builder = WebApplication.CreateBuilder(args);

// Serilog yapılandırmasını appsettings.json'dan okuma
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllersWithViews();

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Add DbContext with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Mail servisi yapılandırması
var mailtrapToken = builder.Configuration["Mailtrap:Token"];
var fromEmail = builder.Configuration["Mailtrap:FromEmail"];
var fromName = builder.Configuration["Mailtrap:FromName"];

if (string.IsNullOrEmpty(mailtrapToken) || string.IsNullOrEmpty(fromEmail) || string.IsNullOrEmpty(fromName))
{
    throw new InvalidOperationException("Mailtrap yapılandırması eksik.");
}

builder.Services.AddSingleton(new MailService(mailtrapToken, fromEmail, fromName));

// Add custom services
builder.Services.AddScoped<QRCodeGeneratorService>(provider =>
    new QRCodeGeneratorService("http://localhost:5139/api/survey"));
builder.Services.AddScoped<HashExistingPasswords>();

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

// QR Kod oluşturma işlemi
using (var scope = app.Services.CreateScope())
{
    var qrCodeService = scope.ServiceProvider.GetRequiredService<QRCodeGeneratorService>();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var passwordHasherService = scope.ServiceProvider.GetRequiredService<HashExistingPasswords>();

    // Existing passwords hash'leme
    try
    {
        await passwordHasherService.HashPasswordsAsync();
        Console.WriteLine("Mevcut şifreler başarıyla hash'lenmiştir.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Şifre hash'leme sırasında hata: {ex.Message}");
    }

    // QR kod oluşturma
    var storeCodes = dbContext.Stores.Select(s => s.StoreCode).ToList();
    foreach (var storeCode in storeCodes)
    {
        try
        {
            qrCodeService.GenerateQRCode(storeCode);
            Console.WriteLine($"{storeCode} için QR kod başarıyla oluşturuldu.");
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

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAllOrigins");
app.UseAuthorization();

// Allow external access
app.Urls.Add("http://0.0.0.0:5139");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
