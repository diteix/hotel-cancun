using System;
using System.Collections.Generic;
using AutoMapper;
using HotelCancun.Application.Mapper;
using HotelCancun.Application.Services.Rooms;
using HotelCancun.Application.Services.Interfaces;
using HotelCancun.Application.Services.Clients;
using HotelCancun.Data.Context;
using HotelCancun.Data.Repository;
using HotelCancun.Domain.Entities;
using HotelCancun.Domain.Repository;
using HotelCancun.Domain.Services.Clients.Query;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using HotelCancun.Domain.Services.Rooms.Query;
using HotelCancun.Domain.Services.Rooms.Command;

namespace HotelCancun.IoC
{
    public static class NativeDependencyInjector
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddScoped<IRoomApplication, RoomApplication>();
            services.AddScoped<IClientApplication, ClientApplication>();

            services.AddAutoMapper(typeof(DtoAndEntityMappingProfile));

            services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());


            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();

            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            var connection = new SqliteConnection(connectionString);

            services.AddDbContext<RoomDbContext>(options =>
                options.UseSqlite(connection)
            );
            services.AddDbContext<ClientDbContext>(options =>
                options.UseSqlite(connection)
            );


            services.AddScoped<IRequestHandler<GetAllRoomsQuery, IList<Room>>, RoomQueryHandler>();
            services.AddScoped<IRequestHandler<GetRoomReservationsQuery, Room>, RoomQueryHandler>();
            services.AddScoped<IRequestHandler<AddReservationCommand>, RoomCommandHandler>();

            services.AddScoped<IRequestHandler<GetClientQuery, Client>, ClientQueryHandle>();
        }

        public static void ConfigureDbContext(this IServiceScopeFactory serviceScopeFactory) {
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                /* var roomDbContext = serviceScope.ServiceProvider.GetService<RoomDbContext>();
                (roomDbContext.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).EnsureCreated(); */

                var roomDbContext = serviceScope.ServiceProvider.GetService<RoomDbContext>();
                roomDbContext.Database.GetService<IDatabaseCreator>().EnsureCreated();
                roomDbContext.Initialize();

                var clientDbContext = serviceScope.ServiceProvider.GetService<ClientDbContext>();
                clientDbContext.Database.GetService<IDatabaseCreator>().EnsureCreated();
                clientDbContext.Initialize();
            }
        }
    }
}
