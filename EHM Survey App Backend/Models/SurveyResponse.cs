using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class SurveyResponse
{
    [Key]
    public int ResponseId { get; set; }

   [ForeignKey("Store")]
    public int StoreId { get; set; }

    public Store? Store { get; set; }

    [ForeignKey("Survey")]
    public int SurveyId { get; set; }

    [Required(ErrorMessage = "Soruların cevaplanması gerekiyor")]
    public string Responses { get; set; } = string.Empty; 
    
    [ForeignKey("Survey")]
    public string Question { get; set; } = string.Empty;

    [ForeignKey("Survey")]
    public string QuestionType { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta adresi girilmesi gerekiyor")]
    [RegularExpression(@"^[a-zA-Z0-9ğüşıöçĞÜŞİÖÇ._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Geçersiz e-posta adresi")]
    public string Email { get; set; } = string.Empty;

    public bool IsVerified { get; set; } = false;

    public string VerificationCode { get; set; } = string.Empty;

    public DateTime SubmissonDate { get; set; } = DateTime.UtcNow;

    public bool Abandoned { get; set; } = false;

    public int NpsScore { get; set; } = 0;

    /// <summary>
    /// [Required(ErrorMessage = "Kullanıcı tarayıcı ve cihaz bilgisi gerekli")]
    /// </summary>
    public string? UserAgent { get; set; } 

    public float CompletionTime { get; set; } = 0;
}