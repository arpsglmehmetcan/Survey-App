using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Survey
{
    [Key]
    public int SurveyId { get; set; } //Anket id'si
    
    [ForeignKey("Store")]
    public int StoreId { get; set; } // Mağaza id'si
    public Store? Store { get; set; }

    [Required(ErrorMessage = "Anket soruları gerekli")]
    public string Question { get; set; } = string.Empty; //sorular
    
    [Required(ErrorMessage = "Anket soru tipi gerekli")]
    public string QuestionType { get; set; } = string.Empty; //soru tipi (radio,text,checkbox)
    
    public bool IsRequired { get; set; } = false; //gereklilik
}
