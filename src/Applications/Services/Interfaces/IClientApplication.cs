using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotelCancun.Application.Dtos.Client;
using HotelCancun.Application.Dtos.Reservation;
using HotelCancun.Application.Dtos.Validation;

namespace HotelCancun.Application.Services.Interfaces 
{
    public interface IClientApplication 
    {
        Task<ValidationDto<ClientDto>> GetAsync(int clientId);

        Task<ValidationDto<IList<ReservationDto>>> GetReservationsAsync(int clientId);

        Task<ValidationDto<object>> CancelReservationAsync(int roomId, int reservationId);

        Task<ValidationDto<ReservationDto>> ModifyReservationAsync(int roomId, int reservationId, DateTime from, DateTime to);
    }
}