using MediatR;
using System.Threading.Tasks;
using System.Threading;
using HotelCancun.Domain.Repository;

namespace HotelCancun.Domain.Services.Clients.Command 
{
    public class ClientCommandHandler : IRequestHandler<DeleteReservationCommand>, IRequestHandler<ModifyReservationCommand>
    {
        private readonly IClientRepository _repository;

        public ClientCommandHandler(IClientRepository repository)
        {
            this._repository = repository;
        }

        public async Task<Unit> Handle(DeleteReservationCommand request, CancellationToken cancellationToken)
        {
            await this._repository.DeleteReservationAsync(request.ClientId, request.ClientRoomId);

            return Unit.Value;
        }

        public async Task<Unit> Handle(ModifyReservationCommand request, CancellationToken cancellationToken)
        {
            await this._repository.DeleteReservationAsync(request.ClientId, request.ClientRoomId);

            return Unit.Value;
        }
    }
}