using MediatR;
using System.Threading.Tasks;
using System.Threading;
using HotelCancun.Domain.Repository;

namespace HotelCancun.Domain.Services.Rooms.Command 
{
    public class RoomCommandHandler : IRequestHandler<AddReservationCommand>
    {
        private readonly IRoomRepository _repository;

        public RoomCommandHandler(IRoomRepository repository)
        {
            this._repository = repository;
        }

        public async Task<Unit> Handle(AddReservationCommand request, CancellationToken cancellationToken)
        {
            await this._repository.AddReservationAsync(request.RoomId, request.ClientId, request.From, request.To);

            return Unit.Value;
        }
    }
}