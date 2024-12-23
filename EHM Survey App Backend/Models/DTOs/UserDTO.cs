namespace EHM_Survey_App_Backend.Models.DTOs
{
    public class UserDTO
    {
        public int UserId { get; set; } // Kullanıcı ID'si
        public string UserMail { get; set; } = string.Empty; // Kullanıcı Adı
        public int StoreId { get; set; } // Mağaza ID'si
    }
}
