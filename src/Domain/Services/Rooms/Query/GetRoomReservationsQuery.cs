using MediatR;
using HotelCancun.Domain.Entities;

namespace HotelCancun.Domain.Services.Rooms.Query 
{
    public class GetRoomReservationsQuery : IRequest<Room> 
    {
        public int RoomId { get; set; }
    }
}