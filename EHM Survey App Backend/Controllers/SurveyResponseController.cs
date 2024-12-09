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

    [HttpPost]
    public async Task<IActionResult> CreateSurveyResponse([FromBody] SurveyResponse request)
    {
        if (!Regex.IsMatch(request.Email, @"^[a-zA-Z0-9ğüşıöçĞÜŞİÖÇ._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
        {
        return BadRequest(new { error = "Geçersiz e-posta adresi." });
        }

        // Soruların cevaplarının kontrolü
        if (request.Responses == null || !request.Responses.Any())
        {
            return BadRequest(new { error = "Soruların cevaplanması gerekiyor." });
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

        // Yeni SurveyResponse oluşturma
        var response = new SurveyResponse
        {
            Email = request.Email,
            StoreId = request.StoreId,
            Responses = request.Responses,
            UserAgent = request.UserAgent,
            VerificationCode = mailResult.VerificationCode ?? string.Empty,
            IsVerified = false,
            SubmissonDate = DateTime.UtcNow
        };

        _context.SurveyResponses.Add(response);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Anket yanıtınız alındı ve doğrulama kodunuz gönderildi." });
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyEmailCode([FromBody] SurveyResponse request)
    {
        // Email alanını kullanarak yanıtı bul
        var response = await _context.SurveyResponses
            .FirstOrDefaultAsync(r => r.Email == request.Email && !r.IsVerified);

        if (response == null)
        {
            return NotFound("Yanıt bulunamadı veya zaten doğrulanmış.");
        }

        // Doğrulama kodlarını karşılaştır
        var mailResult = await _mailService.VerifyCode(request.VerificationCode, response.VerificationCode);
        if (mailResult.IsSuccessful)
        {
            response.IsVerified = true;
            await _context.SaveChangesAsync();
            return Ok("Mail doğrulandı ve sonuç kaydedildi.");
        }

        return BadRequest($"Geçersiz doğrulama kodu: {mailResult.ErrorMessage}");
    }
}
