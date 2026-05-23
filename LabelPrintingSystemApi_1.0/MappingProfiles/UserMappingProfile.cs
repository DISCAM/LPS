using AutoMapper;
using Data.Dtos.User;
using LabelPrintingSystemApi_1._0.Models;

namespace LabelPrintingSystemApi_1._0.MappingProfiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {

            //// wyswietlanie wszystekich 
            ///  wyświtlanie jednego  
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.RoleName, opt => opt.Ignore());

            /// dodawanie
            CreateMap<UserCreateDto, User>();

            /// edycja
            CreateMap<UserEditDto, User>(); 








        }
    }
}
