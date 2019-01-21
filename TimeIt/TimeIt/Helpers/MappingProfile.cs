using AutoMapper;
using TimeIt.Extensions;
using TimeIt.Models;
using TimeIt.ViewModels;

namespace TimeIt.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Interval, IntervalItemViewModel>()
                .ForMember(d => d.Color, opt => opt.MapFrom(s => s.Color.ToHexString(true)))
                .ForMember(d => d.Duration, opt => opt.MapFrom(s => s.Duration))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name))
                .ForMember(d => d.Position, opt => opt.MapFrom(s => s.Position));
            CreateMap<Interval, IntervalListItemViewModel>()
                .ForMember(d => d.Color, opt => opt.MapFrom(s => s.Color.ToHexString(true)))
                .ForMember(d => d.Duration, opt => opt.MapFrom(s => s.Duration))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name))
                .ForMember(d => d.Position, opt => opt.MapFrom(s => s.Position));
            CreateMap<Timer, TimerItemViewModel>()
                .ForMember(d => d.Intervals, opt => opt.MapFrom(s => s.Intervals))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name))
                .ForMember(d => d.Repetitions, opt => opt.MapFrom(s => s.Repetitions));
        }
    }
}
