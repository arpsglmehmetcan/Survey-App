using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;   


[ApiController]
[Route("api/[controller]")]
public class SurveyResponseController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly SmsService _smsService;
    private string GenerateVerificationCode()
    {
        // Doğrulama kodu üretme 
        var code = Random.Shared.Next(10000, 99999).ToString();
        return code;
    }
    public SurveyResponseController(AppDbContext context, SmsService smsService)
    {
        _context = context;
        _smsService = smsService;
    }

    // Anket yanıtını kaydeder ve doğrulama kodu gönderir
    [HttpPost]
    public async Task<IActionResult> CreateSurveyResponse([FromBody] SurveyResponse request)
    {
        // Anket yanıtını veritabanına kaydet
        var response = new SurveyResponse
        {
            // kişisel bilgileri doldurur
            PhoneNumber = request.PhoneNumber
        };
        _context.SurveyResponses.Add(response);
        await _context.SaveChangesAsync();

        // Doğrulama kodu gönder
        var verificationCode = GenerateVerificationCode(); // Rastgele bir doğrulama kodu üret
        var isSent = await _smsService.SendVerificationCode(response.PhoneNumber, verificationCode);

        if (isSent)
        {
            response.VerificationCode = verificationCode;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Anket yanıtınız alındı ve doğrulama kodunuz gönderildi." });
        }
        else
        {
            return StatusCode(500, "Doğrulama kodu gönderilirken bir hata oluştu.");
        }
    }

    // SMS kodunu doğrula
    [HttpPost("verify")]
    public async Task<IActionResult> VerifySmsCode([FromBody] SurveyResponse request)
    {
        var response = await _context.SurveyResponses
            .FirstOrDefaultAsync(r => r.PhoneNumber == request.PhoneNumber && !r.IsVerified);

        if (response == null)
            return NotFound("Yanıt bulunamadı veya zaten doğrulanmış.");

        bool isVerified = _smsService.VerifyCode(response.PhoneNumber, request.VerificationCode);

        if (isVerified)
        {
            response.IsVerified = true;
            await _context.SaveChangesAsync();
            return Ok("SMS doğrulandı ve sonuç kaydedildi.");
        }

        return BadRequest("Geçersiz doğrulama kodu.");
    }
}