using System;

namespace HotelCancun.Application.Dtos.Reservation 
{
    public class ReservationDto 
    {
        public int Id { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public int RoomId { get; set; }
    }
}