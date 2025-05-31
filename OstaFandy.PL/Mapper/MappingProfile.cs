using AutoMapper;
using OstaFandy.DAL.Entities;
using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.Mapper
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            #region User
            CreateMap<User,UserDto>().ReverseMap();
            CreateMap<UserType, UserTypeDto>().ReverseMap();
            CreateMap<User, UserRegesterDto>().ReverseMap();
            CreateMap<User, UserLoginDto>().ReverseMap();
            #endregion
        }

    }
}
