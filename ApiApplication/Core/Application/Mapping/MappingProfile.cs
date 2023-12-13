using ApiApplication.Core.Application.Models.ViewModels;
using ApiApplication.Core.Domain.Entities;
using AutoMapper;
using Google.Protobuf.Collections;
using ProtoDefinitions;
using System.Collections.Generic;
using System.Linq;

namespace ApiApplication.Core.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<showResponse, MovieWrapperViewModel>();
            //CreateMap<RepeatedField<showResponse>, List<MovieViewModel>>();

            CreateMap<MovieEntity, MovieWrapperViewModel>()
                .ForMember(x => x.Id, opt => opt.MapFrom(src => src.ServiceId));

            CreateMap<MovieEntity, MovieViewModel>()
                .ReverseMap();

            CreateMap<MovieWrapperViewModel, MovieEntity>()
                .ForMember(x => x.ServiceId, opt => opt.MapFrom(src => src.Id))
                .ForMember(x => x.Id, opt => opt.Ignore())
                ;

            CreateMap<AuditoriumEntity, AuditoriumViewModel>()
                .ReverseMap();

            CreateMap<ShowtimeEntity, ShowtimeViewModel>().ReverseMap();
            CreateMap<SeatEntity, SeatViewModel>().ReverseMap();

            CreateMap<TicketEntity, TicketViewModel>()
                .ForMember(x => x.Movie, opt => opt.MapFrom(src => src.Showtime.Movie))
                .ForMember(x => x.Auditorium, opt => opt.MapFrom(src => src.Showtime.Auditorium))
                .ForMember(x => x.NumberOfSeats, opt => opt.MapFrom(src => src.TicketSeats.Count))
                .ReverseMap();
        }
    }
}