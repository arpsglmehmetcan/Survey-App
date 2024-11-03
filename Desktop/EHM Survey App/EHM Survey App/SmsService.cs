using System;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Verify.V2.Service;

public class SmsService
{
    private readonly string _accountSid = "AC0207641989a64e367119ae3b8862ea94";
    private readonly string _authToken = "[a051594efe1b2cc77d24a6b4e7109c7a]";
    private readonly string _serviceSid = "VA8f5aad99b864f899aba55ebe582e38df";

    public SmsService()
    {
        TwilioClient.Init(_accountSid, _authToken);
    }

    public class SmsResult
    {
        public bool IsSuccessful { get; set; }
        public string? ErrorMessage { get; set; } = string.Empty;
    }

    public async Task<SmsResult> SendVerificationCode(string PhoneNumber)
    {
        try
        {
            var verification = await VerificationResource.CreateAsync(
                to: PhoneNumber,
                channel: "sms",
                pathServiceSid: _serviceSid
            );

            Console.WriteLine($"Verification SID: {verification.Sid}");
            return new SmsResult
            {
                IsSuccessful = verification.Status == "pending",
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

    public async Task<SmsResult> VerifyCode(string PhoneNumber, string VerificationCode)
    {
        try
        {
            var verificationCheck = await VerificationCheckResource.CreateAsync(
                to: PhoneNumber,
                code: VerificationCode,
                pathServiceSid: _serviceSid
            );

            return new SmsResult
            {
                IsSuccessful = verificationCheck.Status == "approved",
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
