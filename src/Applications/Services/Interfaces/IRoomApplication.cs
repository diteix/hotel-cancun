using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotelCancun.Application.Dtos.Reservation;
using HotelCancun.Application.Dtos.Room;
using HotelCancun.Application.Dtos.Validation;

namespace HotelCancun.Application.Services.Interfaces 
{
    public interface IRoomApplication
    { 
        Task<IList<RoomDto>> GetAllRoomAsync();

        Task<ValidationDto<RoomDto>> GetRoomAndReservationsAsync(int roomId);

        Task<ValidationDto<ReservationDto>> AddReservationAsync(int roomId, int clientId, DateTime from, DateTime to);
    }
}