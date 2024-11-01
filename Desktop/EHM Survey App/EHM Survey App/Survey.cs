using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Survey
{
    [Key] // Primary key olarak belirtin
    public int SurveyId { get; set; }
    
    [ForeignKey("Store")]
    public int StoreId { get; set; } // Mağaza kimliği
    
    public Store? Store { get; set; } // Nullable yapıldı

    [Required(ErrorMessage = "Anket soruları gerekli")]
    public string Question { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Anket soru tipi gerekli")]
    public string QuestionType { get; set; } = string.Empty;
    
    public bool IsRequired { get; set; } = false;
}
