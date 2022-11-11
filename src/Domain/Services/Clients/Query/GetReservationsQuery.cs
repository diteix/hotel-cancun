using MediatR;
using HotelCancun.Domain.Entities;

namespace HotelCancun.Domain.Services.Clients.Query 
{
    public class GetClientAndReservationsQuery : IRequest<Client> 
    {
        public int ClientId { get; set; }
    }
}