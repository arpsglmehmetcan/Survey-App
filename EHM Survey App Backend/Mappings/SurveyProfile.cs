using AutoMapper;
using EHM_Survey_App_Backend.Models;
using EHM_Survey_App_Backend.Models.DTOs;

namespace EHM_Survey_App_Backend.Mappings
{
    public class SurveyProfile : Profile
    {
        public SurveyProfile()
        {
            // Survey -> SurveyDTO
            CreateMap<Survey, SurveyDTO>()
                .ForMember(dest => dest.StoreCode, opt => opt.MapFrom(src => src.Store != null ? src.Store.StoreCode : string.Empty))
                .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store != null ? src.Store.StoreName : string.Empty));
            
            // SurveyDTO -> Survey
            CreateMap<SurveyDTO, Survey>()
                .ForMember(dest => dest.Store, opt => opt.Ignore()); // Store ilişkilendirilmiş bir nesne olduğu için ignore ediliyor
            
            // SurveyResponse -> SurveyResponseDTO
            CreateMap<SurveyResponse, SurveyResponseDTO>();

            // SurveyOrderUpdateDTO
            CreateMap<SurveyOrderUpdateDTO, SurveyOrderUpdateDTO>().ReverseMap(); // Doğru sınıf kullanımı
        }
    }
}
