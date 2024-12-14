using RestSharp;
using System;
using System.Threading.Tasks;
using Serilog;

public class MailService
{
    private readonly string _mailtrapToken;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public MailService(string mailtrapToken, string fromEmail, string fromName)
    {
        _mailtrapToken = mailtrapToken;
        _fromEmail = fromEmail;
        _fromName = fromName;
    }

    public class MailResult
    {
        public bool IsSuccessful { get; set; }
        public string? ErrorMessage { get; set; }
        public string? VerificationCode { get; set; }
    }

    private string GenerateVerificationCode()
    {
        Random random = new Random();
        return random.Next(100000, 999999).ToString();
    }

    public async Task<MailResult> SendVerificationCode(string toEmail)
    {
        try
        {
            string verificationCode = GenerateVerificationCode();

            var client = new RestClient("https://sandbox.api.mailtrap.io/api/send/3264193");
            var request = new RestRequest();
            request.AddHeader("Authorization", $"Bearer {_mailtrapToken}");
            request.AddHeader("Content-Type", "application/json");

            string payload = $@"
        {{
            ""from"": {{ ""email"": ""{_fromEmail}"", ""name"": ""{_fromName}"" }},
            ""to"": [{{ ""email"": ""{toEmail}"" }}],
            ""subject"": ""Verification Code: {verificationCode}"",
            ""text"": ""Your verification code is: {verificationCode}""
        }}";

            request.AddParameter("application/json", payload, ParameterType.RequestBody);
            var response = await client.ExecutePostAsync(request);

            if (response.IsSuccessful)
            {
                // Başarılı durumlarda
                Log.Information("Mail başarıyla gönderildi: {@ResponseContent}", response.Content);
                return new MailResult
                {
                    IsSuccessful = true,
                    VerificationCode = verificationCode
                };
            }

            // Başarısız durumlarda
            Log.Error("Mail gönderim hatası: {@ResponseContent}", response.Content);
            return new MailResult
            {
                IsSuccessful = false,
                ErrorMessage = response.Content
            };
        }
        catch (Exception ex)
        {
            Log.Error("Mail gönderimi sırasında bir hata oluştu: {ErrorMessage}", ex.Message);
            return new MailResult
            {
                IsSuccessful = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
