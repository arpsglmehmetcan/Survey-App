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

    public SurveyController(AppDbContext context)
    {
        _context = context;
    }

    // Mağaza koduna göre anketleri getirir
    [HttpGet("{storeCode}")]
    public async Task<ActionResult<IEnumerable<Survey>>> GetSurveyByStore(string storeCode)
    {
        var surveys = await _context.Surveys
            .Where(s => s.StoreCode == storeCode)
            .ToListAsync();

        if (surveys == null || !surveys.Any())
        {
            return NotFound();
        }

        return Ok(surveys);  // Daha iyi yanıt kodu için Ok() kullanılabilir
    }

    // Anket cevabını kaydeder
    [HttpPost]
    public async Task<ActionResult<SurveyResponse>> SubmitSurveyResponse(SurveyResponse response)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        response.SubmissonDate = DateTime.UtcNow;
        _context.SurveyResponses.Add(response);
        await _context.SaveChangesAsync();

        // SMS doğrulama adımı eklenebilir

        return CreatedAtAction(nameof(GetSurveyByStore), new { storeCode = response.StoreCode }, response);
    }
}
