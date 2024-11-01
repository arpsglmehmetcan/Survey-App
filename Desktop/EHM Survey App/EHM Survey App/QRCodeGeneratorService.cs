using System;
using System.Diagnostics;
using System.IO;

public class QRCodeGeneratorService
{
    private readonly string _baseUrl;

    public QRCodeGeneratorService(string baseUrl)
    {
        _baseUrl = baseUrl; // Örneğin: "http://192.168.1.33:5139/survey"
    }

    public void GenerateQRCode(string storeCode)
    {
        // Python betiğini çalıştırma
        string pythonScript = "generate_qr.py";
        string pythonPath = @"C:\Users\mehme\AppData\Local\Programs\Python\Python312\python.exe"; // Python'un kurulu olduğu tam yolu belirtin
        string arguments = $"{pythonScript} {storeCode}";

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
        string filePath = Path.Combine("wwwroot", "qrcodes", $"{storeCode}_qrcode.png");

        // wwwroot/qrcodes dizini yoksa oluştur
        Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException("Directory adı alınamadı."));
    }
}
