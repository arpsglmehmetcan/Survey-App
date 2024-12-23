using AutoMapper;
using EHM_Survey_App_Backend.Models; 
using EHM_Survey_App_Backend.Models.DTOs; 

namespace EHM_Survey_App_Backend.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserRole, UserDTO>();
        }
    }
}
