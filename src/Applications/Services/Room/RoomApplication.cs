using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using HotelCancun.Application.Dtos.Reservation;
using HotelCancun.Application.Dtos.Room;
using HotelCancun.Application.Dtos.Validation;
using HotelCancun.Application.Services.Interfaces;
using HotelCancun.Domain.Entities;
using HotelCancun.Domain.Services.Rooms.Command;
using HotelCancun.Domain.Services.Rooms.Query;
using MediatR;

namespace HotelCancun.Application.Services.Rooms 
{
    public class RoomApplication : IRoomApplication
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public RoomApplication(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<IList<RoomDto>> GetAllRoomAsync()
        {
            return _mapper.Map<IList<RoomDto>>(await _mediator.Send(new GetAllRoomsQuery()));
        }

        public async Task<ValidationDto<RoomDto>> GetRoomAndReservationsAsync(int roomId)
        {
            var room = await GetRoomReservationAsync(roomId);

            if (room == null)
            {
                return new ValidationDto<RoomDto>("Room not found");
            }

            return new ValidationDto<RoomDto>()
            {
                Value = _mapper.Map<RoomDto>(room)
            };
        }

        public async Task<ValidationDto<ReservationDto>> AddReservationAsync(int roomId, int clientId, DateTime from, DateTime to)
        {
            var room = await GetRoomReservationAsync(roomId);

            if (room == null)
            {
                return new ValidationDto<ReservationDto>("Room not found");
            }

            var reservations = _mapper.Map<IList<ReservationDto>>(room.ClientRooms);

            var currentReservation = new ReservationDto()
            {
                From = from,
                To = to,
                RoomId = roomId
            };

            var validation = ValidateReservationService.Validate(reservations, currentReservation);

            if (validation.IsValid)
            {
                var command = new AddReservationCommand()
                {
                    RoomId = roomId,
                    ClientId = clientId,
                    From = from,
                    To = to
                };

                await _mediator.Send(command);
            }

            return validation;
        }

        private async Task<Room> GetRoomReservationAsync(int roomId)
        {
            var query = new GetRoomReservationsQuery()
            {
                RoomId = roomId
            };

            return await _mediator.Send(query);
        }
    }
}