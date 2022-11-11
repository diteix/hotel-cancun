using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotelCancun.Domain.Entities;

namespace HotelCancun.Domain.Repository
{
    public interface IRoomRepository
    {
        Task<IList<Room>> GetAllAsync();

        Task<Room> GetRoomAndReservationsAsync(int roomId);

        Task AddReservationAsync(int roomId, int clientId, DateTime from, DateTime to);
    }
}