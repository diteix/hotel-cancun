using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelCancun.Data.Context;
using HotelCancun.Domain.Entities;
using HotelCancun.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace HotelCancun.Data.Repository
{
    public class ClientRepository : IClientRepository
    {
        private ClientDbContext _context;

        public ClientRepository(ClientDbContext context)
        {
            _context = context;
        }
        
        public async Task<Client> GetAsync(int clientId)
        {
            return await _context.Clients.SingleOrDefaultAsync(s => s.Id == clientId);
        }

        public async Task<Client> GetClientAndReservationsAsync(int clientId)
        {
            return await _context.Clients.Include(s => s.ClientRooms).SingleOrDefaultAsync(s => s.Id == clientId);
        }

        public async Task DeleteReservationAsync(int clientId, int clientRoomId)
        {
            var client = await GetClientAndReservationsAsync(clientId);

            var clientRoom = client.ClientRooms.Single(s => s.Id == clientRoomId);

            client.ClientRooms.Remove(clientRoom);

            await _context.SaveChangesAsync();
        }

        public async Task ModifyReservationAsync(int clientId, int clientRoomId, DateTime from, DateTime to)
        {
            var client = await GetClientAndReservationsAsync(clientId);

            var clientRoom = client.ClientRooms.Single(s => s.Id == clientRoomId);

            clientRoom.From = from;
            clientRoom.To = to;

            _context.Clients.Update(client);

            await _context.SaveChangesAsync();
        }
    }
}