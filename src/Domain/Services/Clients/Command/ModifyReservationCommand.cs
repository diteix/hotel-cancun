using MediatR;
using System;

namespace HotelCancun.Domain.Services.Clients.Command 
{
    public class ModifyReservationCommand : IRequest 
    {
        public int ClientId { get; set; }
        public int ClientRoomId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}