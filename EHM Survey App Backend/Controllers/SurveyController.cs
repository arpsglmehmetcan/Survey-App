using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class SurveyController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly QRCodeGeneratorService _qrCodeGeneratorService;

    public SurveyController(AppDbContext context, QRCodeGeneratorService qrCodeGeneratorService)
    {
        _context = context;
        _qrCodeGeneratorService = qrCodeGeneratorService;
    }

    [HttpGet("get-survey/{StoreCode}")]
    public async Task<ActionResult<IEnumerable<Survey>>> GetSurveyByStore(string StoreCode)
    {
        var store = await _context.Stores.FirstOrDefaultAsync(s => s.StoreCode == StoreCode);
        if (store == null)
        {
            return NotFound(new { message = "Bu mağaza koduna sahip mağaza bulunamadı." });
        }

        var surveys = await _context.Surveys
            .Where(s => s.StoreId == store.StoreId) // IsActive filtresi yok, bu yüzden tüm soruları alır
            .OrderBy(s => s.Order)
            .ToListAsync();

        if (surveys == null || !surveys.Any())
        {
            return NotFound(new { message = "Bu mağaza için anket sorusu bulunamadı." });
        }

        return Ok(surveys);
    }

    [HttpGet("get-active-survey/{StoreCode}")]
    public async Task<ActionResult<IEnumerable<Survey>>> GetActiveSurveyByStore(string StoreCode)
    {
        var store = await _context.Stores.FirstOrDefaultAsync(s => s.StoreCode == StoreCode);
        if (store == null)
        {
            return NotFound(new { message = "Bu mağaza koduna sahip mağaza bulunamadı." });
        }

        var surveys = await _context.Surveys
            .Where(s => s.StoreId == store.StoreId && s.IsActive) // Yalnızca aktif soruları al
            .OrderBy(s => s.Order) // Sıralama
            .ToListAsync();

        if (surveys == null || !surveys.Any())
        {
            return NotFound(new { message = "Bu mağaza için aktif anket sorusu bulunamadı." });
        }

        return Ok(surveys);
    }

    [HttpPost("submit-response")]
    public async Task<ActionResult<SurveyResponse>> SubmitSurveyResponse(SurveyResponse Response)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var store = await _context.Stores.FindAsync(Response.StoreId);
        if (store == null)
        {
            return NotFound("Mağaza bulunamadı.");
        }

        Response.SubmissonDate = DateTime.UtcNow;
        _context.SurveyResponses.Add(Response);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSurveyByStore), new { StoreCode = store.StoreCode }, Response);
    }

    [HttpGet("generate-qrcode/{StoreCode}")]
    public async Task<IActionResult> GenerateQRCode(string StoreCode)
    {
        var store = await _context.Stores.FirstOrDefaultAsync(s => s.StoreCode == StoreCode);
        if (store == null)
        {
            return NotFound("Mağaza bulunamadı.");
        }

        _qrCodeGeneratorService.GenerateQRCode(store.StoreCode);
        return Ok($"QR kod {store.StoreCode}_qrcode.png olarak oluşturuldu ve kaydedildi.");
    }

    [HttpPost("add-survey")]
    public async Task<IActionResult> AddSurvey([FromBody] Survey newSurvey)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Geçersiz veri");
        }

        _context.Surveys.Add(newSurvey);
        await _context.SaveChangesAsync();
        return Ok("Soru başarıyla eklendi");
    }

    [HttpPut("update-survey/{id}")]
    public async Task<IActionResult> UpdateSurvey(int id, [FromBody] Survey updatedSurvey)
    {
        var existingSurvey = await _context.Surveys.FindAsync(id);
        if (existingSurvey == null)
        {
            return NotFound("Soru bulunamadı");
        }

        existingSurvey.Question = updatedSurvey.Question;
        existingSurvey.QuestionType = updatedSurvey.QuestionType;
        existingSurvey.QuestionOptions = updatedSurvey.QuestionOptions;
        existingSurvey.IsRequired = updatedSurvey.IsRequired;
        existingSurvey.Order = updatedSurvey.Order;
        existingSurvey.IsActive = updatedSurvey.IsActive;

        await _context.SaveChangesAsync();
        return Ok("Soru başarıyla güncellendi");
    }

    [HttpDelete("delete-survey/{id}")]
    public async Task<IActionResult> DeleteSurvey(int id)
    {
        var survey = await _context.Surveys.FindAsync(id);
        if (survey == null)
        {
            return NotFound("Soru bulunamadı");
        }

        _context.Surveys.Remove(survey);
        await _context.SaveChangesAsync();
        return Ok("Soru başarıyla silindi");
    }

    [HttpPatch("update-survey-order")]
    public async Task<IActionResult> UpdateSurveyOrder([FromBody] List<int> surveyOrder)
    {
        for (int i = 0; i < surveyOrder.Count; i++)
        {
            var survey = await _context.Surveys.FindAsync(surveyOrder[i]);
            if (survey != null)
            {
                survey.Order = i + 1; // Yeni sıralama
            }
        }

        await _context.SaveChangesAsync();
        return Ok("Sıralama güncellendi");
    }

}
