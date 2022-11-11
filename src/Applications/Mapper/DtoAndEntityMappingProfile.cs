using AutoMapper;
using HotelCancun.Application.Dtos.Client;
using HotelCancun.Application.Dtos.Reservation;
using HotelCancun.Application.Dtos.Room;
using HotelCancun.Domain.Entities;

namespace HotelCancun.Application.Mapper
{
    public class DtoAndEntityMappingProfile : Profile
    {
        public DtoAndEntityMappingProfile()
        {
            CreateMap<ClientRoom, ReservationDto>();
            CreateMap<RoomDto, Room>().ForMember(s => s.ClientRooms, opt => opt.MapFrom(s => s.Reservations)).ReverseMap();
            CreateMap<ClientDto, Client>().ReverseMap();
        }
    }
}