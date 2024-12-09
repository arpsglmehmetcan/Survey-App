using Microsoft.AspNetCore.Mvc; // ControllerBase, IActionResult, HttpPost vb. için
using Microsoft.EntityFrameworkCore; // Entity Framework Core uzantı metodları için
using Newtonsoft.Json; // JSON işlemleri için

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
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (string.IsNullOrEmpty(request.Responses))
        {
            return BadRequest(new { error = "Soruların cevaplanması gerekiyor." });
        }

        Dictionary<string, object> responses;
        try
        {
            responses = JsonConvert.DeserializeObject<Dictionary<string, object>>(request.Responses);
            if (responses == null || !responses.Any())
            {
                return BadRequest(new { error = "Soruların cevaplanması gerekiyor." });
            }
        }
        catch (JsonException ex)
        {
            return BadRequest(new { error = $"Soruların formatı hatalı: {ex.Message}" });
        }

        var store = await _context.Stores.FirstOrDefaultAsync(s => s.StoreId == request.StoreId);
        if (store == null)
        {
            return NotFound("Mağaza bulunamadı.");
        }

        var mailResult = await _mailService.SendVerificationCode(request.Email);
        if (!mailResult.IsSuccessful)
        {
            return StatusCode(500, new { error = $"Doğrulama kodu gönderilirken bir hata oluştu: {mailResult.ErrorMessage}" });
        }

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
        var response = await _context.SurveyResponses
            .FirstOrDefaultAsync(r => r.Email == request.Email && !r.IsVerified);

        if (response == null)
        {
            return NotFound(new { error = "Yanıt bulunamadı veya zaten doğrulanmış." });
        }

        if (response.VerificationCode != request.VerificationCode)
        {
            return BadRequest(new { error = "Geçersiz doğrulama kodu." });
        }

        response.IsVerified = true;
        await _context.SaveChangesAsync();
        return Ok(new { message = "Mail doğrulandı ve sonuç kaydedildi." });
    }
}

