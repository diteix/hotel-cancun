using System.Collections.Generic;
using HotelCancun.Application.Dtos.Reservation;

namespace HotelCancun.Application.Dtos.Room 
{
    public class RoomDto 
    {
        public int Id { get; set; }

        public string Number { get; set; }

        public IList<ReservationDto> Reservations { get; set; }
    }
}