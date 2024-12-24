using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EHM_Survey_App_Backend.Models;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("get-all-users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _context.UserRole.ToListAsync();
        return Ok(users);
    }

    [HttpPost("add-user")]
    public async Task<IActionResult> AddUser([FromBody] UserRole user)
    {
        if (string.IsNullOrEmpty(user.UserMail) || string.IsNullOrEmpty(user.Password))
        {
            return BadRequest(new { message = "E-posta ve şifre gereklidir." });
        }

        user.Password = PasswordHasher.HashPassword(user.Password);
        _context.UserRole.Add(user);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Kullanıcı başarıyla eklendi." });
    }

    [HttpPut("update-user/{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserRole updatedUser)
    {
        var user = await _context.UserRole.FindAsync(id);
        if (user == null)
        {
            return NotFound(new { message = "Kullanıcı bulunamadı." });
        }

        // Gelen e-posta ve StoreID'yi güncelle
        user.UserMail = updatedUser.UserMail;
        user.StoreIds = updatedUser.StoreIds;

        // Gelen şifre boş değilse şifreyi güncelle
        if (!string.IsNullOrEmpty(updatedUser.Password))
        {
            user.Password = PasswordHasher.HashPassword(updatedUser.Password);
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Kullanıcı başarıyla güncellendi." });
    }

    [HttpDelete("delete-user/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.UserRole.FindAsync(id);
        if (user == null)
        {
            return NotFound(new { message = "Kullanıcı bulunamadı." });
        }

        _context.UserRole.Remove(user);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Kullanıcı başarıyla silindi." });
    }
}
