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

    // Mağaza koduna göre anketleri getirir
    [HttpGet("get-surveys/{storeCode}")]
    public async Task<ActionResult<IEnumerable<Survey>>> GetSurveyByStore(string storeCode)
    {
        var store = await _context.Stores.FirstOrDefaultAsync(s => s.StoreCode == storeCode);
        if (store == null)
        {
            return NotFound("Bu mağaza koduna sahip mağaza bulunamadı.");
        }

        var surveys = await _context.Surveys
            .Where(s => s.StoreId == store.StoreId)
            .ToListAsync();

        if (surveys == null || !surveys.Any())
        {
            return NotFound("Bu mağaza için anket sorusu bulunamadı.");
        }

        return Ok(surveys);
    }

    // Anket cevabını kaydeder
    [HttpPost("submit-response")]
    public async Task<ActionResult<SurveyResponse>> SubmitSurveyResponse(SurveyResponse response)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var store = await _context.Stores.FindAsync(response.StoreId);
        if (store == null)
        {
            return NotFound("Mağaza bulunamadı.");
        }

        response.SubmissonDate = DateTime.UtcNow;
        _context.SurveyResponses.Add(response);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSurveyByStore), new { storeCode = store.StoreCode }, response);
    }

    // QR kod oluşturur
    [HttpGet("generate-qrcode/{storeCode}")]
    public async Task<IActionResult> GenerateQRCode(string storeCode)
    {
        var store = await _context.Stores.FirstOrDefaultAsync(s => s.StoreCode == storeCode);
        if (store == null)
        {
            return NotFound("Mağaza bulunamadı.");
        }

        _qrCodeGeneratorService.GenerateQRCode(store.StoreCode);
        return Ok($"QR kod {store.StoreCode}_qrcode.png olarak oluşturuldu ve kaydedildi.");
    }
}
