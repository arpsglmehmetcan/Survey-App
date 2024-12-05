using System;
using System.Diagnostics;
using System.IO;

public class QRCodeGeneratorService
{
    private readonly string _baseUrl;

    public QRCodeGeneratorService(string baseUrl)
    {
        _baseUrl = baseUrl; // Bu URL, front-end uygulamanızın ana adresi olmalı, örneğin "http://192.168.1.5:3000"
    }

    public void GenerateQRCode(string StoreCode)
    {
        // Python betiğini çalıştırma
        string pythonScript = "generate_qr.py"; // Python betiğinin adı
        string pythonPath = @"C:\Users\mehme\AppData\Local\Microsoft\WindowsApps\python.exe"; // Python çalıştırıcı yolu
        string arguments = $"\"{pythonScript}\" \"{StoreCode}\""; // Sadece mağaza kodunu gönderiyoruz

        ProcessStartInfo start = new ProcessStartInfo
        {
            FileName = pythonPath,
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        try
        {
            using (Process process = Process.Start(start) ?? throw new InvalidOperationException("Python işlemi başlatılamadı."))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd(); // Python betiğinin standart çıktısını okuma
                    Console.WriteLine(result); // Terminale yazdır
                }

                string errors = process.StandardError.ReadToEnd(); // Python betiğinin hata çıktısını okuma
                if (!string.IsNullOrEmpty(errors))
                {
                    Console.WriteLine("Hata: " + errors);
                }
            }

            // QR kodunun kaydedileceği dosya yolunu oluştur
            string filePath = Path.Combine("wwwroot", "qrcodes", $"{StoreCode}_qrcode.png");
            Console.WriteLine($"QR kod başarıyla oluşturuldu ve şu dizine kaydedildi: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"QR kod oluşturma sırasında hata: {ex.Message}");
        }
    }
}
