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
        // Retrieve StoreId using storeCode
        var store = await _context.Stores.FirstOrDefaultAsync(s => s.StoreId == request.StoreId);
        if (store == null)
        {
            return NotFound("Mağaza bulunamadı.");
        }

        var Response = new SurveyResponse
        {
            PhoneNumber = request.PhoneNumber,
            StoreId = request.StoreId,  
            Response = request.Response,
            UserAgent = request.UserAgent,
            IsVerified = false
        };

        _context.SurveyResponses.Add(Response);
        await _context.SaveChangesAsync();

        var smsResult = await _smsService.SendVerificationCode(Response.PhoneNumber);

        if (smsResult.IsSuccessful)
        {
            return Ok(new { message = "Anket yanıtınız alındı ve doğrulama kodunuz gönderildi." });
        }
        else
        {
            return StatusCode(500, $"Doğrulama kodu gönderilirken bir hata oluştu: {smsResult.ErrorMessage}");
        }
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifySmsCode([FromBody] SurveyResponse request)
    {
        var Response = await _context.SurveyResponses
            .FirstOrDefaultAsync(r => r.PhoneNumber == request.PhoneNumber && !r.IsVerified);

        if (Response == null)
            return NotFound("Yanıt bulunamadı veya zaten doğrulanmış.");

        var smsResult = await _smsService.VerifyCode(Response.PhoneNumber, request.VerificationCode);

        if (smsResult.IsSuccessful)
        {
            Response.IsVerified = true;
            await _context.SaveChangesAsync();
            return Ok("SMS doğrulandı ve sonuç kaydedildi.");
        }

        return BadRequest($"Geçersiz doğrulama kodu: {smsResult.ErrorMessage}");
    }
}
