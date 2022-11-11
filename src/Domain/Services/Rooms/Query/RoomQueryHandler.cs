using System.Collections.Generic;
using MediatR;
using HotelCancun.Domain.Entities;
using System.Threading.Tasks;
using System.Threading;
using HotelCancun.Domain.Repository;

namespace HotelCancun.Domain.Services.Rooms.Query 
{
    public class RoomQueryHandler : IRequestHandler<GetAllRoomsQuery, IList<Room>>, IRequestHandler<GetRoomReservationsQuery, Room>
    {
        private readonly IRoomRepository _repository;

        public RoomQueryHandler(IRoomRepository repository)
        {
            this._repository = repository;
        }

        public async Task<IList<Room>> Handle(GetAllRoomsQuery request, CancellationToken cancellationToken)
        {
            return await this._repository.GetAllAsync();
        }

        public async Task<Room> Handle(GetRoomReservationsQuery request, CancellationToken cancellationToken)
        {
            return await this._repository.GetRoomAndReservationsAsync(request.RoomId);
        }
    }
}