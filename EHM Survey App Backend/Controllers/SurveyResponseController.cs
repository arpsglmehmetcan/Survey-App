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
        // Gelen verileri kontrol etmek için log ekleyelim
        Console.WriteLine($"Gelen Email: {request.Email}");
        Console.WriteLine($"Gelen VerificationCode: {request.VerificationCode}");
        Console.WriteLine($"Database'deki VerificationCode: {response.VerificationCode}");
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyEmailCode([FromBody] SurveyResponse request)
    {
        // Gelen verileri kontrol etmek için log ekleyelim
        Console.WriteLine($"Gelen Email: {request.Email}");
        Console.WriteLine($"Gelen VerificationCode: {request.VerificationCode}");

        // Gelen verilerin boş olup olmadığını kontrol et
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.VerificationCode))
        {
            Console.WriteLine("E-posta veya doğrulama kodu boş.");
            return BadRequest(new { error = "E-posta ve doğrulama kodu gerekli." });
        }

        // Database'den kayıt çek
        var response = await _context.SurveyResponses
            .FirstOrDefaultAsync(r => r.Email == request.Email && !r.IsVerified);

        if (response == null)
        {
            Console.WriteLine("Yanıt bulunamadı veya zaten doğrulanmış.");
            return NotFound(new { error = "Yanıt bulunamadı veya zaten doğrulanmış." });
        }

        // Doğrulama kodlarını karşılaştır
        Console.WriteLine($"Database'deki VerificationCode: {response.VerificationCode}");

        if (response.VerificationCode != request.VerificationCode)
        {
            Console.WriteLine("Doğrulama kodu uyuşmuyor.");
            return BadRequest(new { error = "Geçersiz doğrulama kodu." });
        }

        // Doğrulama başarılı, yanıtı güncelle
        response.IsVerified = true;
        await _context.SaveChangesAsync();

        Console.WriteLine("Doğrulama başarılı, yanıt güncellendi.");
        return Ok(new { message = "Mail doğrulandı ve sonuç kaydedildi." });
    }



}
