using System;
using System.Diagnostics;
using System.IO;

public class QRCodeGeneratorService
{
    private readonly string _baseUrl;

    public QRCodeGeneratorService(string baseUrl)
    {
        _baseUrl = baseUrl; // Bu URL front-end uygulamanızın ana adresi olmalı, örneğin "http://192.168.1.33:3000"
    }

    public void GenerateQRCode(string StoreCode)
    {
        // Önce tam URL'yi oluşturuyoruz
        string url = $"{_baseUrl}/survey/{StoreCode}";

        // Python betiğini çalıştırma
        string pythonScript = "generate_qr.py";
        string pythonPath = @"C:\Users\mehme\AppData\Local\Programs\Python\Python312\python.exe";
        string arguments = $"{pythonScript} \"{url}\""; // Python betiğine tam URL'yi geçiyoruz

        ProcessStartInfo start = new ProcessStartInfo
        {
            FileName = pythonPath,
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using (Process process = Process.Start(start) ?? throw new InvalidOperationException("Process başlatılamadı."))
        {
            using (StreamReader reader = process.StandardOutput)
            {
                string result = reader.ReadToEnd();
                Console.WriteLine(result);
            }

            string errors = process.StandardError.ReadToEnd();
            if (!string.IsNullOrEmpty(errors))
            {
                Console.WriteLine("Hata: " + errors);
            }
        }

        // QR kodunu kaydetmek için dosya yolu oluşturma
        string filePath = Path.Combine("wwwroot", "qrcodes", $"{StoreCode}_qrcode.png");
        Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException("Directory adı alınamadı."));
    }
}
