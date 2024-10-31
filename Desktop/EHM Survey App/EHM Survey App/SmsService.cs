using System;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Verify.V2.Service;

public class SmsService
{
    private readonly string _accountSid = "AC0207641989a64e367119ae3b8862ea94"; // Twilio Account SID
    private readonly string _authToken = "[a051594efe1b2cc77d24a6b4e7109c7a]"; // Twilio Auth Token
    private readonly string _serviceSid = "VA8f5aad99b864f899aba55ebe582e38df"; // Twilio Service SID (Verify Service)

    public SmsService()
    {
        TwilioClient.Init(_accountSid, _authToken);
    }

    // Doğrulama kodu gönderim sonucu için SmsResult sınıfı
    public class SmsResult
    {
        public bool IsSuccessful { get; set; }
        public string? ErrorMessage { get; set; } = string.Empty; // Nullable hale getirildi
    }


    // Doğrulama kodu gönder
    public async Task<SmsResult> SendVerificationCode(string phoneNumber)
    {
        try
        {
            var verification = await VerificationResource.CreateAsync(
                to: phoneNumber,
                channel: "sms",
                pathServiceSid: _serviceSid
            );

            Console.WriteLine($"Verification SID: {verification.Sid}");
            return new SmsResult
            {
                IsSuccessful = verification.Status == "beklemede", // pending -> beklemede
                ErrorMessage = null
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hata oluştu: {ex.Message}");
            return new SmsResult
            {
                IsSuccessful = false,
                ErrorMessage = ex.Message
            };
        }
    }

    // Doğrulama kodunu kontrol et
    public async Task<SmsResult> VerifyCode(string phoneNumber, string verificationCode)
    {
        try
        {
            var verificationCheck = await VerificationCheckResource.CreateAsync(
                to: phoneNumber,
                code: verificationCode,
                pathServiceSid: _serviceSid
            );

            return new SmsResult
            {
                IsSuccessful = verificationCheck.Status == "onaylandı", // approved -> onaylandı
                ErrorMessage = null
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hata oluştu: {ex.Message}");
            return new SmsResult
            {
                IsSuccessful = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
