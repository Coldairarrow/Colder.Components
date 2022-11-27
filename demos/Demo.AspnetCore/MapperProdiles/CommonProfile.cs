using AutoMapper;
using Demo.AspnetCore.Dtos;
using Demo.AspnetCore.Entities;

namespace Invest.Api.MapperProdiles;

public class CommonProfile : Profile
{
    public CommonProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<User, UserExtendDto>().ReverseMap();
    }
}
