using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly AppDbContext _context;

    public LoginController(AppDbContext context)
    {
        _context = context;
    }

    // Kullanıcı Giriş Kontrolü
    [HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
{
    if (loginRequest == null || string.IsNullOrEmpty(loginRequest.UserName) || string.IsNullOrEmpty(loginRequest.Password))
    {
        return BadRequest(new { success = false, message = "Kullanıcı adı ve şifre gereklidir." });
    }

    // Kullanıcıyı kontrol et
    var user = await _context.UserRole
        .Where(u => u.UserName == loginRequest.UserName)
        .FirstOrDefaultAsync();

    // Kullanıcı adı kontrolü
    if (user == null)
    {
        return NotFound(new { success = false, message = "Kayıtlı kullanıcı bulunamadı." });
    }

    // Şifre kontrolü
    if (user.Password != loginRequest.Password)
    {
        return Unauthorized(new { success = false, message = "Kullanıcı adı veya şifre yanlış." });
    }

    // Giriş başarılı
    return Ok(new
    {
        success = true,
        StoreId = user.StoreId,
        UserName = user.UserName,
        message = "Giriş başarılı."
    });
}


    // Giriş Bilgilerini Taşıyan Sınıf
    public class LoginRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
