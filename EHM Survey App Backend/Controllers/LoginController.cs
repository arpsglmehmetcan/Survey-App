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
        if (loginRequest == null || string.IsNullOrEmpty(loginRequest.UserMail) || string.IsNullOrEmpty(loginRequest.Password))
        {
            return BadRequest(new { success = false, message = "E-posta ve şifre gereklidir." });
        }

        // Kullanıcıyı kontrol et
        var user = await _context.UserRole
            .FirstOrDefaultAsync(u => u.UserMail == loginRequest.UserMail);

        // Kullanıcı adı kontrolü
        if (user == null)
        {
            return NotFound(new { success = false, message = "Kayıtlı kullanıcı bulunamadı." });
        }

        // Şifre kontrolü
        if (!PasswordHasher.VerifyPassword(loginRequest.Password, user.Password))
        {
            return Unauthorized(new { success = false, message = "E-posta veya şifre yanlış." });
        }

        // Giriş başarılı
        return Ok(new
        {
            success = true,
            StoreIds = user.StoreIds,
            UserMail = user.UserMail,
            message = "Giriş başarılı."
        });
    }

    // Giriş Bilgilerini Taşıyan Sınıf
    public class LoginRequest
    {
        public string UserMail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
