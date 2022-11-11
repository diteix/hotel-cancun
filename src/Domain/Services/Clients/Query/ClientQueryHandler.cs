using System.Collections.Generic;
using MediatR;
using HotelCancun.Domain.Entities;
using System.Threading.Tasks;
using System.Threading;
using HotelCancun.Domain.Repository;

namespace HotelCancun.Domain.Services.Clients.Query 
{
    public class ClientQueryHandle : IRequestHandler<GetClientQuery, Client>, IRequestHandler<GetClientAndReservationsQuery, Client>
    {
        private readonly IClientRepository _repository;

        public ClientQueryHandle(IClientRepository repository)
        {
            _repository = repository;
        }

        public async Task<Client> Handle(GetClientQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAsync(request.ClientId);
        }

        public async Task<Client> Handle(GetClientAndReservationsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetClientAndReservationsAsync(request.ClientId);
        }
    }
}