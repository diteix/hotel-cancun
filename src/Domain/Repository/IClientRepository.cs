using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotelCancun.Domain.Entities;

namespace HotelCancun.Domain.Repository
{
    public interface IClientRepository
    {
        Task<Client> GetAsync(int clientId);

        Task<Client> GetClientAndReservationsAsync(int clientId);

        Task DeleteReservationAsync(int clientId, int clientRoomId);

        Task ModifyReservationAsync(int clientId, int clientRoomId, DateTime from, DateTime to);
    }
}