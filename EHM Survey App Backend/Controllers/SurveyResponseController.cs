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
        if (string.IsNullOrWhiteSpace(request.Responses) || !request.Responses.Any())
        {
            return BadRequest(new { error = "Soruların tamamını cevaplamanız gerekiyor." });
        }

        // Mağaza kontrolü
        var store = await _context.Stores.FirstOrDefaultAsync(s => s.StoreId == request.StoreId);
        if (store == null)
        {
            return NotFound("Mağaza bulunamadı.");
        }

        // Email alanını kullanarak doğrulama kodu gönderimi
        var mailResult = await _mailService.SendVerificationCode(request.Email);
        if (!mailResult.IsSuccessful)
        {
            return StatusCode(500, $"Doğrulama kodu gönderilirken bir hata oluştu: {mailResult.ErrorMessage}");
        }

        // Temp olarak doğrulama kodunu kaydet
        var response = new SurveyResponse
        {
            Email = request.Email,
            StoreId = request.StoreId,
            VerificationCode = mailResult.VerificationCode ?? string.Empty,
            IsVerified = false,
            SubmissonDate = DateTime.UtcNow
        };

        _context.SurveyResponses.Add(response);
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

        // Database'den doğrulama kodunu kontrol et
        var response = await _context.SurveyResponses
            .FirstOrDefaultAsync(r => r.Email == request.Email && !r.IsVerified);

        if (response == null || response.VerificationCode != request.VerificationCode)
        {
            return BadRequest(new { error = "Geçersiz doğrulama kodu." });
        }

        // Doğrulama başarılı, yanıtları kaydet
        response.IsVerified = true;
        response.Responses = request.Responses; // Cevapları ekle
        await _context.SaveChangesAsync();

        return Ok(new { message = "Doğrulama başarılı! Yanıtlar kaydedildi." });
    }
}

