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
    public class RoomRepository : IRoomRepository
    {
        private readonly RoomDbContext _context;

        public RoomRepository(RoomDbContext context)
        {
            _context = context;
        }

        public async Task<IList<Room>> GetAllAsync()
        {
            return await _context.Rooms.ToListAsync();
        }

        public async Task<Room> GetRoomAndReservationsAsync(int roomId)
        {
            return await _context.Rooms.Include(s => s.ClientRooms).SingleOrDefaultAsync(s => s.Id == roomId);
        }

        public async Task AddReservationAsync(int roomId, int clientId, DateTime from, DateTime to) 
        {
            var clientRoom = new ClientRoom()
            {
                ClientId = clientId,
                From = from,
                To = to
            };
            
            var room = await GetRoomAndReservationsAsync(roomId);

            room.ClientRooms.Add(clientRoom);

            await _context.SaveChangesAsync();
        }
    }
}