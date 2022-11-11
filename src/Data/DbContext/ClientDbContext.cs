using HotelCancun.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelCancun.Data.Context
{
    public class ClientDbContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }

        public ClientDbContext(DbContextOptions<ClientDbContext> options) : base(options)
        { 
            
        }

        public void Initialize()
        {
            if (Clients.Find(1) != null) 
            {
                return;
            }

            Clients.Add(new Client() {
                Id = 1,
                Username = "diego",
                Password = "123"
            });

            SaveChanges();
        }
    }
}