namespace EHM_Survey_App_Backend.Models.DTOs
{
    public class SurveyDTO
    {
        public int SurveyId { get; set; } // Anket ID'si
        public string Question { get; set; } = string.Empty; // Soru metni
        public string QuestionType { get; set; } = string.Empty; // Soru tipi (radio, text, checkbox)
        public string? QuestionOptions { get; set; } // Çoğul seçenekler (JSON formatında)
        public int StoreId { get; set; } // Mağaza ID'si
        public string? StoreCode { get; set; } // Mağaza kodu
        public string? StoreName { get; set; } // Mağaza adı
        public bool IsRequired { get; set; } // Sorunun zorunlu olup olmadığını belirtir
        public int Order { get; set; } // Sorunun sırası
        public bool IsActive { get; set; } // Sorunun aktif olup olmadığını belirtir
    }
}
