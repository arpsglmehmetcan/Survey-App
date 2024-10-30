using System;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Verify.V2.Service;

public class SmsService
{
    private readonly string _accountSid = "AC71b5d5fec00c39a77fe44f105af34edd"; // Twilio Account SID
    private readonly string _authToken = "[AuthToken]"; // Twilio Auth Token
    private readonly string _serviceSid = "VA11c801d896feab2ea5cdc5b23e35b128"; // Twilio Service SID (Verify Service)

    public SmsService()
    {
        TwilioClient.Init(_accountSid, _authToken);
    }

    public async Task<bool> SendVerificationCode(string phoneNumber)
    {
        try
        {
            var verification = await VerificationResource.CreateAsync(
                to: phoneNumber,
                channel: "sms",
                pathServiceSid: _serviceSid
            );

            Console.WriteLine($"Verification SID: {verification.Sid}");
            return verification.Status == "pending";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hata oluştu: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> VerifyCode(string phoneNumber, string verificationCode)
    {
        try
        {
            var verificationCheck = await VerificationCheckResource.CreateAsync(
                to: phoneNumber,
                code: verificationCode,
                pathServiceSid: _serviceSid
            );

            return verificationCheck.Status == "approved";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hata oluştu: {ex.Message}");
            return false;
        }
    }
}
