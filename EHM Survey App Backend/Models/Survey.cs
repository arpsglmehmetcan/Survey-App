using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Survey
{
    [Key]
    public int SurveyId { get; set; } // Anket ID'si

    [ForeignKey("Store")]
    public int StoreId { get; set; } // Mağaza ID'si
    public Store? Store { get; set; }

    [Required(ErrorMessage = "Soru gereklidir")]
    public string Question { get; set; } = string.Empty; // Soru metni

    [Required(ErrorMessage = "Soru tipi gereklidir")]
    public string QuestionType { get; set; } = string.Empty; // Soru tipi (radio, text, checkbox)

    public string? QuestionOptions { get; set; } // Çoğul seçenekler (JSON formatında)

    public bool IsRequired { get; set; } = false; // Sorunun zorunlu olup olmadığını belirtir

    public int Order { get; set; } = 0; // Sorunun sırası

    public bool IsActive { get; set; } = true; // Sorunun aktif olup olmadığını belirtir
}