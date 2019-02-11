using AutoMapper;
using TimeIt.Extensions;
using TimeIt.Models;
using TimeIt.ViewModels;
using Xamarin.Forms;

namespace TimeIt.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Interval, IntervalItemViewModel>()
                .ForMember(d => d.Color, opt => opt.MapFrom(s => s.Color.ToHexString(true)))
                .ReverseMap()
                .ForMember(d => d.Color, opt => opt.MapFrom(s => Color.FromHex(s.Color)));

            CreateMap<Interval, IntervalListItemViewModel>()
                .ForMember(d => d.Color, opt => opt.MapFrom(s => s.Color.ToHexString(true)))
                .ReverseMap()
                .ForMember(d => d.Color, opt => opt.MapFrom(s => Color.FromHex(s.Color)));
            CreateMap<Timer, TimerItemViewModel>().ReverseMap();

            CreateMap<IntervalListItemViewModel, Interval>()
                .ForMember(d => d.Color, opt => opt.MapFrom(s => Color.FromHex(s.Color)))
                .ReverseMap()
                .ForMember(d => d.Color, opt => opt.MapFrom(s => s.Color.ToHexString(true)));
        }
    }
}
