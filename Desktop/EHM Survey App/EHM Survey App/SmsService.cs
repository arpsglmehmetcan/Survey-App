using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

public class SmsService
{
    private readonly string _apiKey = "YOUR_NETGSM_API_KEY"; // Netgsm API anahtarınız
    private readonly string _baseUri = "https://api.netgsm.com.tr/sms"; //Netgsm SMS API'sinin temel adresi

    public async Task<bool> SendVerificationCode(string phoneNumber, string code)
    {
        try
        {
            using (var client = new HttpClient())
            {
                // Netgsm'e gönderilecek veriyi hazırla
                var content = new StringContent($"user={_apiKey}&message={code}&gsmno={phoneNumber}");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                // Netgsm'e post isteği gönder
                var response = await client.PostAsync(_baseUri, content);

                // İstek başarılı mı kontrol et
                if (response.IsSuccessStatusCode)
                {
                    return true; // Başarılı
                }
                else
                {
                    // Hata oluştu, detaylı hata mesajı al
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Hata: {errorContent}");
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            // Genel bir hata oluştu mesajı
            Console.WriteLine($"Hata oluştu: {ex.Message}");
            return false;
        }
    }

    internal bool VerifyCode(string phoneNumber, string verificationCode)
    {
        throw new NotImplementedException();
    }
}