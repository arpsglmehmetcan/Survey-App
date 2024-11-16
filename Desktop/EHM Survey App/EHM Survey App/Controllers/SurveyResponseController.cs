using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class SurveyResponseController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly SmsService _smsService;

    public SurveyResponseController(AppDbContext context, SmsService smsService)
    {
        _context = context;
        _smsService = smsService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSurveyResponse([FromBody] SurveyResponse request)
    {
        var store = await _context.Stores.FirstOrDefaultAsync(s => s.StoreId == request.StoreId);
        if (store == null)
            return NotFound("Mağaza bulunamadı.");

        var smsResult = await _smsService.SendVerificationCode(request.PhoneNumber);
        if (!smsResult.IsSuccessful)
        {
            return StatusCode(500, $"Doğrulama kodu gönderilirken bir hata oluştu: {smsResult.ErrorMessage}");
        }

        var response = new SurveyResponse
        {
            PhoneNumber = request.PhoneNumber,
            StoreId = request.StoreId,
            Response = request.Response,
            UserAgent = request.UserAgent,
            VerificationCode = smsResult.VerificationCode ?? string.Empty,
            IsVerified = false,
            SubmissonDate = DateTime.UtcNow
        };

        _context.SurveyResponses.Add(response);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Anket yanıtınız alındı ve doğrulama kodunuz gönderildi." });
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifySmsCode([FromBody] SurveyResponse request)
    {
        var response = await _context.SurveyResponses
            .FirstOrDefaultAsync(r => r.PhoneNumber == request.PhoneNumber && !r.IsVerified);

        if (response == null)
        {
            return NotFound("Yanıt bulunamadı veya zaten doğrulanmış.");
        }

        var smsResult = await _smsService.VerifyCode(request.VerificationCode, response.VerificationCode);
        if (smsResult.IsSuccessful)
        {
            response.IsVerified = true;
            await _context.SaveChangesAsync();
            return Ok("SMS doğrulandı ve sonuç kaydedildi.");
        }

        return BadRequest($"Geçersiz doğrulama kodu: {smsResult.ErrorMessage}");
    }
}
