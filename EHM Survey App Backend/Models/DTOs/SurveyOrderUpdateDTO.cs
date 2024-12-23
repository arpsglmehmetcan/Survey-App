namespace EHM_Survey_App_Backend.Models.DTOs
{
    public class SurveyOrderUpdateDTO
    {
        public int SurveyId { get; set; } // Güncellenen anketin ID'si
        public int Order { get; set; } // Yeni sıra numarası
    }
}
