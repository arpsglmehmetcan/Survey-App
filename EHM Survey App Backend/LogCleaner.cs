using System;
using System.IO;
using System.Linq;

public static class LogCleaner
{
    public static void CleanUpOldLogs(string logsDirectory, int maxLogFilesToKeep)
    {
        try
        {
            var logFiles = Directory.GetFiles(logsDirectory, "log-*.txt");

            if (logFiles.Length > maxLogFilesToKeep)
            {
                var filesToDelete = logFiles
                    .Select(f => new FileInfo(f))
                    .OrderByDescending(f => f.CreationTime)
                    .Skip(maxLogFilesToKeep);

                foreach (var file in filesToDelete)
                {
                    Console.WriteLine($"Siliniyor: {file.FullName}");
                    file.Delete();
                }
            }
            else
            {
                Console.WriteLine("Silinecek dosya bulunamadı.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Log temizleme sırasında hata: {ex.Message}");
        }
    }
}
