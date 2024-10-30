using System.ComponentModel.DataAnnotations;

public class Survey
{
    [Key] // Primary key olarak belirtin
    public int SurveyId { get; set; }
    
    [Required(ErrorMessage = "Mağaza kodu gerekli")]
    public string StoreCode { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Anket soruları gerekli")]
    public string Question { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Anket soru tipi gerekli")]
    public string QuestionType { get; set; } = string.Empty;
    
    public bool IsRequired { get; set; } = false;
}
