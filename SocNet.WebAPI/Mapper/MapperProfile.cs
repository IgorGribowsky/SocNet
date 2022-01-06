using AutoMapper;
using SocNet.WebAPI.Models;
using SocNet.Core.Entities;
using SocNet.Services.Dtos;

namespace SocNet.WebAPI.Mapper;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<InputPostData, Post>()
            .ForMember(
            dest => dest.ParentPostId,
            source => source.MapFrom(source => source.ParentPostId > 0 ? source.ParentPostId : null));
        CreateMap<UserSignupModel, SignupDto>();
    }
}
