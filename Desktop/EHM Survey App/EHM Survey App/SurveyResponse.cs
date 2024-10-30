using System;
using System.ComponentModel.DataAnnotations;

public class SurveyResponse
{
    [Key] // Primary key olarak belirtin
    public int ResponseId { get; set; } 
    
    public int SurveyId { get; set; }
    
    [Required(ErrorMessage = "Mağaza kodu gerekiyor")]
    public string StoreCode { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Soruların cevaplanması gerekiyor")]
    public string Response { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Telefon numarası girilmesi gerekiyor")]
    [Phone(ErrorMessage = "Geçersiz telefon numarası")]
    public string PhoneNumber { get; set; } = string.Empty;
    
    public bool IsVerified { get; set; } = false;
    
    public string VerificationCode { get; set; } = string.Empty;
    
    public DateTime SubmissonDate { get; set; } = DateTime.UtcNow;
    
    public bool isAbandoned { get; set; } = false;
    
    public int NpsScore { get; set; } = 0;
    
    [Required(ErrorMessage = "Kullanıcı tarayıcı ve cihaz bilgisi gerekli")]
    public string UserAgent { get; set; } = string.Empty;
    
    public float CompletionTime { get; set; } = 0;
}
