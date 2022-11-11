using MediatR;

namespace HotelCancun.Domain.Services.Clients.Command 
{
    public class DeleteReservationCommand : IRequest 
    {
        public int ClientId { get; set; }
        public int ClientRoomId { get; set; }
    }
}