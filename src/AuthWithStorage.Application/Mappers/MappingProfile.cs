using AuthWithStorage.Application.DTOs;
using AuthWithStorage.Domain.Entities;
using AutoMapper;

namespace AuthWithStorage.Application.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, UserResponse>().ReverseMap();
        }
    }
}
