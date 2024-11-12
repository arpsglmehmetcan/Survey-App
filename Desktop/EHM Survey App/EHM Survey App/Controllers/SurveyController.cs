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

    [HttpGet("get-surveys/{StoreCode}")]
    public async Task<ActionResult<IEnumerable<Survey>>> GetSurveyByStore(string StoreCode)
    {
        var store = await _context.Stores.FirstOrDefaultAsync(s => s.StoreCode == StoreCode);
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

    [HttpPost("submit-Response")]
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
}
