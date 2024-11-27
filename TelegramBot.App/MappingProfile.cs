using AutoMapper;
using TelegramBot.App.DTO;
using TelegramBot.Domain.Models;

namespace TelegramBot.App;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Person, PersonDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Name + " " + src.SecondName + " " + src.ThirdName));

        CreateMap<PersonDto, Person>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FullName.Split()[0]))
            .ForMember(dest => dest.SecondName, opt => opt.MapFrom(src => src.FullName.Split()[1]))
            .ForMember(dest => dest.ThirdName, opt => opt.MapFrom(src => src.FullName.Split()[2]));
    }
}