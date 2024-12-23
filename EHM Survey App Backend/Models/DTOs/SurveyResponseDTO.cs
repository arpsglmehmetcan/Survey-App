namespace EHM_Survey_App_Backend.Models.DTOs
{
    public class SurveyResponseDTO
    {
        public int ResponseId { get; set; } // Yanıt ID'si
        public int StoreId { get; set; } // Mağaza ID'si
        public int SurveyId { get; set; } // Anket ID'si
        public string Responses { get; set; } = string.Empty; // Cevaplar
        public string Email { get; set; } = string.Empty; // Kullanıcının e-posta adresi
        public bool IsVerified { get; set; } = false; // Yanıt doğrulama durumu
    }
}
