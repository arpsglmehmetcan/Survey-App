using Microsoft.AspNetCore.Mvc; // ControllerBase, IActionResult, HttpPost vb. için
using Microsoft.EntityFrameworkCore; // Entity Framework Core uzantı metodları için
using System.Text.RegularExpressions; // Regex sınıfı için eklendi
using AutoMapper;
using EHM_Survey_App_Backend.Models.DTOs;

[ApiController]
[Route("api/[controller]")]
public class SurveyResponseController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly MailService _mailService;
    private readonly IMapper _mapper;

    public SurveyResponseController(AppDbContext context, MailService mailService, IMapper mapper)
    {
        _context = context;
        _mailService = mailService;
        _mapper = mapper;
    }

    [HttpPost("send-code")]
    public async Task<IActionResult> SendVerificationCode([FromBody] SurveyResponseDTO request)
    {
        // Validate email
        if (!Regex.IsMatch(request.Email, @"^[a-zA-Z0-9ğüşıöçĞÜŞİÖÇ._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
        {
            return BadRequest(new { error = "Geçersiz e-posta adresi." });
        }

        // Validate responses
        if (string.IsNullOrWhiteSpace(request.Responses))
        {
            return BadRequest(new { error = "Soruların tamamını cevaplamanız gerekiyor." });
        }

        // Check if store exists
        var store = await _context.Stores.FirstOrDefaultAsync(s => s.StoreId == request.StoreId);
        if (store == null)
        {
            return NotFound(new { message = "Mağaza bulunamadı." });
        }

        // Parse responses
        var parsedResponses = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(request.Responses)
                              ?? new Dictionary<int, string>();

        // Send verification code
        var mailResult = await _mailService.SendVerificationCode(request.Email);
        if (!mailResult.IsSuccessful)
        {
            return StatusCode(500, $"Doğrulama kodu gönderilirken bir hata oluştu: {mailResult.ErrorMessage}");
        }

        // Create SurveyResponse records for each response
        var surveyResponses = new List<SurveyResponse>();
        foreach (var response in parsedResponses)
        {
            var survey = await _context.Surveys.FirstOrDefaultAsync(s => s.SurveyId == response.Key);
            if (survey == null)
            {
                continue; // Skip if survey not found
            }

            surveyResponses.Add(new SurveyResponse
            {
                Email = request.Email,
                StoreId = request.StoreId,
                Responses = response.Value,
                Question = survey.Question,
                QuestionType = survey.QuestionType,
                VerificationCode = mailResult.VerificationCode ?? string.Empty,
                IsVerified = false,
                SubmissonDate = DateTime.UtcNow
            });
        }

        _context.SurveyResponses.AddRange(surveyResponses);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Doğrulama kodu gönderildi." });
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyEmailCode([FromBody] VerificationRequestDTO request)
    {
        // E-posta ve doğrulama kodu kontrolü
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.VerificationCode))
        {
            return BadRequest(new { error = "E-posta ve doğrulama kodu gerekli." });
        }

        // Veritabanından ilgili doğrulanmamış yanıtları getir
        var responses = await _context.SurveyResponses
            .Where(r => r.Email == request.Email && !r.IsVerified)
            .ToListAsync();

        // Yanıt yoksa hata döndür
        if (!responses.Any())
        {
            return BadRequest(new { error = "Doğrulama için kayıt bulunamadı." });
        }

        // İlk kaydın doğrulama kodunu kontrol et
        if (responses.First().VerificationCode != request.VerificationCode)
        {
            return BadRequest(new { error = "Geçersiz doğrulama kodu." });
        }

        // Yanıtların verilerini JSON olarak çözümle ve kaydı güncelle
        if (!string.IsNullOrWhiteSpace(request.Responses))
        {
            try
            {
                var parsedResponses = !string.IsNullOrEmpty(request.Responses)
                    ? Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(request.Responses)
                    : new Dictionary<int, string>();


                foreach (var response in responses)
                {
                    // Yanıt ID'sine göre değer güncelle
                    if (parsedResponses != null && parsedResponses.TryGetValue(response.SurveyId, out var answer))
                    {
                        response.Responses = answer;
                    }

                    // Yanıtı doğrulanmış olarak işaretle
                    response.IsVerified = true;
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Yanıt verileri işlenirken bir hata oluştu: {ex.Message}" });
            }
        }
        else
        {
            // Yanıtlar boşsa sadece doğrulama durumunu güncelle
            foreach (var response in responses)
            {
                response.IsVerified = true;
            }
        }

        await _context.SaveChangesAsync();

        return Ok(new { message = "Doğrulama başarılı! Yanıtlar kaydedildi." });
    }
}