namespace EHM_Survey_App_Backend.Models.DTOs
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public string UserMail { get; set; } = string.Empty; 
        public List<int> StoreIds { get; set; } = new List<int>(); // Tek veya çoklu mağaza için
    }
}