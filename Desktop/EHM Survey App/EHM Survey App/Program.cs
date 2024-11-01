using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add custom services
builder.Services.AddScoped<SmsService>();
builder.Services.AddScoped<QRCodeGeneratorService>(provider => new QRCodeGeneratorService("http://192.168.1.33:5139/survey"));

var app = builder.Build();

// QR kodları üretmek için bir servis oluşturun ve veritabanından mağaza kodlarını alın
using (var scope = app.Services.CreateScope())
{
    var qrCodeService = scope.ServiceProvider.GetRequiredService<QRCodeGeneratorService>();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Veritabanından mağaza kodlarını çekiyoruz
    var storeCodes = dbContext.Stores.Select(s => s.StoreCode).ToList(); // 'Stores' ve 'StoreCode' veritabanınızda mağaza kodlarını tuttuğunuz tablo ve sütun isimleri olmalı

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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Ağdan erişim sağlamak için tüm IP'lerden gelen istekleri kabul edecek şekilde ayarlıyoruz.
app.Urls.Add("http://0.0.0.0:5139");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
