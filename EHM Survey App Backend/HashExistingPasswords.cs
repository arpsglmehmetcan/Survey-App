using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EHM_Survey_App_Backend
{
    public class HashExistingPasswords
    {
        private readonly AppDbContext _context;

        public HashExistingPasswords(AppDbContext context)
        {
            _context = context;
        }

        public async Task HashPasswordsAsync()
        {
            var users = await _context.UserRole.ToListAsync();
            foreach (var user in users)
            {
                // Eğer şifre zaten hashlenmişse (örneğin "$2b$" ile başlıyorsa), atla
                if (!string.IsNullOrEmpty(user.Password) && user.Password.StartsWith("$2"))
                {
                    continue;
                }
                
                // Hash işlemini sadece henüz hashlenmemiş şifreler için yap
                user.Password = PasswordHasher.HashPassword(user.Password);
            }
            await _context.SaveChangesAsync();
        }
    }
}
