using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using System;
using System.Threading.Tasks;

public class SmsService
{
    private readonly string _accountSid = "AC0207641989a64e367119ae3b8862ea94";
    private readonly string _authToken = "4f34ae76dacd1d9193c9a1f8cbb1f0d7";
    private readonly string _serviceSid = "VAadc9a492c8210038c928a1c28362973c";

    public SmsService()
    {
        TwilioClient.Init(_accountSid, _authToken);
    }

    public class SmsResult
    {
        public bool IsSuccessful { get; set; }
        public string? ErrorMessage { get; set; }
        public string? VerificationCode { get; set; }
    }

    // 6 haneli rastgele bir doğrulama kodu oluşturur
    private string GenerateVerificationCode()
    {
        Random random = new Random();
        return random.Next(100000, 999999).ToString();
    }

    public async Task<SmsResult> SendVerificationCode(string phoneNumber)
    {
        try
        {
            // 6 haneli doğrulama kodu oluştur
            string verificationCode = GenerateVerificationCode();

            // SMS gönderimi için mesaj ayarları
            var messageOptions = new CreateMessageOptions(new PhoneNumber(phoneNumber))
            {
                Body = $"Doğrulama kodunuz: {verificationCode}"
            };

            var message = await MessageResource.CreateAsync(messageOptions);
            Console.WriteLine($"SMS gönderildi: {message.Sid}");

            return new SmsResult
            {
                IsSuccessful = true,
                VerificationCode = verificationCode
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

    public Task<SmsResult> VerifyCode(string inputCode, string actualCode)
    {
        // Kullanıcının girdiği kod ile gönderilen kodu karşılaştır
        if (inputCode == actualCode)
        {
            return Task.FromResult(new SmsResult { IsSuccessful = true });
        }

        return Task.FromResult(new SmsResult
        {
            IsSuccessful = false,
            ErrorMessage = "Doğrulama kodu hatalı."
        });
    }
}
