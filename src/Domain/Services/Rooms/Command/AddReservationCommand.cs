using MediatR;
using System;

namespace HotelCancun.Domain.Services.Rooms.Command 
{
    public class AddReservationCommand : IRequest 
    {
        public int RoomId { get; set; }
        public int ClientId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}