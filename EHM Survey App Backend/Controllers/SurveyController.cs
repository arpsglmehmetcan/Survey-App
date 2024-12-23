using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using EHM_Survey_App_Backend.Models.DTOs;

[ApiController]
[Route("api/[controller]")]
public class SurveyController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly QRCodeGeneratorService _qrCodeGeneratorService;
    private readonly IMapper _mapper;

    public SurveyController(AppDbContext context, QRCodeGeneratorService qrCodeGeneratorService, IMapper mapper)
    {
        _context = context;
        _qrCodeGeneratorService = qrCodeGeneratorService;
        _mapper = mapper;
    }

    [HttpGet("get-survey/{StoreCode}")]
    public async Task<ActionResult<IEnumerable<SurveyDTO>>> GetSurveyByStore(string StoreCode)
    {
        var store = await _context.Stores.FirstOrDefaultAsync(s => s.StoreCode == StoreCode);
        if (store == null)
        {
            return NotFound(new { message = "Bu mağaza koduna sahip mağaza bulunamadı." });
        }

        var surveys = await _context.Surveys
            .Where(s => s.StoreId == store.StoreId)
            .OrderBy(s => s.Order)
            .ToListAsync();

        if (surveys == null || !surveys.Any())
        {
            return NotFound(new { message = "Bu mağaza için anket sorusu bulunamadı." });
        }

        var surveyDTOs = _mapper.Map<List<SurveyDTO>>(surveys);
        return Ok(surveyDTOs);
    }

    [HttpGet("get-active-survey/{StoreCode}")]
    public async Task<ActionResult<IEnumerable<SurveyDTO>>> GetActiveSurveyByStore(string StoreCode)
    {
        var store = await _context.Stores.FirstOrDefaultAsync(s => s.StoreCode == StoreCode);
        if (store == null)
        {
            return NotFound(new { message = "Bu mağaza koduna sahip mağaza bulunamadı." });
        }

        var surveys = await _context.Surveys
            .Where(s => s.StoreId == store.StoreId && s.IsActive)
            .OrderBy(s => s.Order)
            .ToListAsync();

        if (surveys == null || !surveys.Any())
        {
            return NotFound(new { message = "Bu mağaza için aktif anket sorusu bulunamadı." });
        }

        var surveyDTOs = _mapper.Map<List<SurveyDTO>>(surveys);
        return Ok(surveyDTOs);
    }

    [HttpPost("submit-response")]
    public async Task<IActionResult> SubmitSurveyResponse([FromBody] SurveyResponseDTO responseDTO)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var store = await _context.Stores.FindAsync(responseDTO.StoreId);
        if (store == null)
        {
            return NotFound("Mağaza bulunamadı.");
        }

        var response = _mapper.Map<SurveyResponse>(responseDTO);
        response.SubmissonDate = DateTime.UtcNow;

        _context.SurveyResponses.Add(response);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSurveyByStore), new { StoreCode = store.StoreCode }, responseDTO);
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
    public async Task<IActionResult> AddSurvey([FromBody] SurveyDTO surveyDTO)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Geçersiz veri");
        }

        var newSurvey = _mapper.Map<Survey>(surveyDTO);
        _context.Surveys.Add(newSurvey);
        await _context.SaveChangesAsync();
        return Ok("Soru başarıyla eklendi");
    }

    [HttpPut("update-survey/{id}")]
    public async Task<IActionResult> UpdateSurvey(int id, [FromBody] SurveyDTO surveyDTO)
    {
        var existingSurvey = await _context.Surveys.FindAsync(id);
        if (existingSurvey == null)
        {
            return NotFound("Soru bulunamadı");
        }

        _mapper.Map(surveyDTO, existingSurvey); // Map updated DTO fields to the existing survey
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
    public async Task<IActionResult> UpdateSurveyOrder([FromBody] List<SurveyOrderUpdateDTO> surveyOrderDTOs)
    {
        if (surveyOrderDTOs == null || !surveyOrderDTOs.Any())
        {
            return BadRequest("Geçersiz veri: Sıralama listesi boş.");
        }

        foreach (var dto in surveyOrderDTOs)
        {
            var survey = await _context.Surveys.FindAsync(dto.SurveyId);
            if (survey != null)
            {
                survey.Order = dto.Order;
            }
        }

        await _context.SaveChangesAsync();
        return Ok("Sıralama başarıyla güncellendi.");
    }

    public class SurveyOrderUpdate
    {
        public int SurveyId { get; set; }
        public int Order { get; set; }
    }


}
