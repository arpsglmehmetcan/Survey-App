namespace EHM_Survey_App_Backend.Models.DTOs
{
    public class VerificationRequestDTO
    {
        public string Email { get; set; } = string.Empty; // Kullanıcının e-posta
        public string VerificationCode { get; set; } = string.Empty; // Kullanıcının gönderdiği doğrulama kodu
        public string Responses { get; set; } = string.Empty; // Kullanıcının yanıtları (JSON formatında)
    }
}
