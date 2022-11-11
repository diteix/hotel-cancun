using HotelCancun.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelCancun.Data.Context
{
    public class RoomDbContext : DbContext
    {
        public DbSet<Room> Rooms { get; set; }

        public RoomDbContext(DbContextOptions<RoomDbContext> options) : base(options)
        { 

        }

        public void Initialize()
        {
            if (Rooms.Find(1) != null) 
            {
                return;
            }

            Rooms.Add(new Room() {
                Id = 1,
                Number = "01"
            });

            SaveChanges();
        }
    }
}