using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace EHM_Survey_App_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StoreController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// StoreId'ye göre StoreCode döndürür.
        /// </summary>
        /// <param name="StoreId"></param>
        /// <returns>StoreCode</returns>
        [HttpGet("get-storecode/{StoreId}")]
        public async Task<IActionResult> GetStoreCode(int StoreId)
        {
            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.StoreId == StoreId);

            if (store == null)
            {
                return NotFound(new { message = "Mağaza bulunamadı." });
            }

            return Ok(new { StoreCode = store.StoreCode });
        }
    }
}
