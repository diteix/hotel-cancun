using System;
using System.Collections.Generic;
using System.Linq;
using HotelCancun.Application.Dtos.Reservation;
using HotelCancun.Application.Dtos.Validation;

namespace HotelCancun.Application.Services
{
    public static class ValidateReservationService
    {
        public static ValidationDto<ReservationDto> Validate(IList<ReservationDto> reservations, ReservationDto currentReservation)
        {
            // the stay can’t be longer than 3 days
            if ((currentReservation.To - currentReservation.From).Days > 3)
            {
                return CreateResponseObject("Reservation can't be longer than 3 days.", currentReservation);
            }

            // can’t be reserved more than 30 days in advance.
            if (currentReservation.From > DateTime.Today.AddDays(30))
            {
                return CreateResponseObject("Can't be reserved more than 30 days in advance.", currentReservation);
            }

            // All reservations start at least the next day of booking,
            if (currentReservation.From <= DateTime.Today) 
            {
                return CreateResponseObject("All reservations start at least the next day of booking.", currentReservation);
            }

            // Check if room is not reserved
            foreach (var reservation in reservations)
            {
                if (IsBewteenTwoDates(currentReservation.From, reservation.From, reservation.To) 
                    || IsBewteenTwoDates(currentReservation.To, reservation.From, reservation.To))
                {
                    return CreateResponseObject("Room already reserverd in the requested dates.", currentReservation);
                }
            }

            return new ValidationDto<ReservationDto>();
        }

        private static ValidationDto<ReservationDto> CreateResponseObject(string message, ReservationDto value)
        {
            return new ValidationDto<ReservationDto>(message)
            {
                Value = value
            };
        }

        private static bool IsBewteenTwoDates(DateTime date, DateTime start, DateTime end)
        {
            return date >= start && date <= end;
        }
    }
}