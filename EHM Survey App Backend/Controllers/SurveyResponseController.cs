using Microsoft.AspNetCore.Mvc; // ControllerBase, IActionResult, HttpPost vb. için
using Microsoft.EntityFrameworkCore; // Entity Framework Core uzantı metodları için
using System.Text.RegularExpressions; // Regex sınıfı için eklendi

[ApiController]
[Route("api/[controller]")]
public class SurveyResponseController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly MailService _mailService;

    public SurveyResponseController(AppDbContext context, MailService mailService)
    {
        _context = context;
        _mailService = mailService;
    }

    [HttpPost("send-code")]
    public async Task<IActionResult> SendVerificationCode([FromBody] SurveyResponse request)
    {
        // Email doğrulama
        if (!Regex.IsMatch(request.Email, @"^[a-zA-Z0-9ğüşıöçĞÜŞİÖÇ._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
        {
            return BadRequest(new { error = "Geçersiz e-posta adresi." });
        }

        // Soruların kontrolü
        if (string.IsNullOrWhiteSpace(request.Responses))
        {
            return BadRequest(new { error = "Soruların tamamını cevaplamanız gerekiyor." });
        }

        // Mağaza kontrolü
        var store = await _context.Stores.FirstOrDefaultAsync(s => s.StoreId == request.StoreId);
        if (store == null)
        {
            return NotFound("Mağaza bulunamadı.");
        }

        // Yanıtların işlenmesi
        var responses = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(request.Responses)
                        ?? new Dictionary<int, string>();

        // Doğrulama kodu gönder
        var mailResult = await _mailService.SendVerificationCode(request.Email);
        if (!mailResult.IsSuccessful)
        {
            return StatusCode(500, $"Doğrulama kodu gönderilirken bir hata oluştu: {mailResult.ErrorMessage}");
        }

        foreach (var response in responses)
        {
            var survey = await _context.Surveys.FirstOrDefaultAsync(s => s.SurveyId == response.Key);
            if (survey == null)
            {
                continue; // Survey bulunamazsa geç
            }

            // Her cevap için bir SurveyResponse kaydı oluştur
            var surveyResponse = new SurveyResponse
            {
                Email = request.Email,
                StoreId = request.StoreId,
                Responses = response.Value,
                Question = survey.Question,
                QuestionType = survey.QuestionType,
                VerificationCode = mailResult.VerificationCode ?? string.Empty,
                IsVerified = false,
                SubmissonDate = DateTime.UtcNow
            };

            _context.SurveyResponses.Add(surveyResponse);
        }

        await _context.SaveChangesAsync();

        return Ok(new { message = "Doğrulama kodu gönderildi." });
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyEmailCode([FromBody] SurveyResponse request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.VerificationCode))
        {
            return BadRequest(new { error = "E-posta ve doğrulama kodu gerekli." });
        }

        // Doğrulama kodunu ve e-posta adresini kontrol et
        var responses = await _context.SurveyResponses
            .Where(r => r.Email == request.Email && !r.IsVerified)
            .ToListAsync();

        if (!responses.Any())
        {
            return BadRequest(new { error = "Doğrulama için kayıt bulunamadı." });
        }

        // İlk kaydın doğrulama kodunu kontrol et
        if (responses.First().VerificationCode != request.VerificationCode)
        {
            return BadRequest(new { error = "Geçersiz doğrulama kodu." });
        }

        // JSON formatındaki yanıtları işleme
        var parsedResponses = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(request.Responses);
        foreach (var response in responses)
        {
            if (parsedResponses.TryGetValue(response.SurveyId, out var answer))
            {
                response.Responses = answer;
            }

            // Doğrulama başarılı olduğu için `IsVerified` alanını true yap
            response.IsVerified = true;
        }

        await _context.SaveChangesAsync();

        return Ok(new { message = "Doğrulama başarılı! Yanıtlar kaydedildi." });
    }

}
