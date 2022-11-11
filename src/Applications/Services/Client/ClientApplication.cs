using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HotelCancun.Application.Dtos.Client;
using HotelCancun.Application.Dtos.Reservation;
using HotelCancun.Application.Dtos.Validation;
using HotelCancun.Application.Services.Interfaces;
using HotelCancun.Domain.Entities;
using HotelCancun.Domain.Services.Clients.Command;
using HotelCancun.Domain.Services.Clients.Query;
using MediatR;

namespace HotelCancun.Application.Services.Clients
{
    public class ClientApplication : IClientApplication
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public ClientApplication(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<ValidationDto<ClientDto>> GetAsync(int clientId) 
        {
            var client = await GetClientAsync(clientId);

            if (client == null)
            {
                return new ValidationDto<ClientDto>("Client not found");
            }

            return new ValidationDto<ClientDto>()
            {
                Value = _mapper.Map<ClientDto>(client)
            };
        }

        public async Task<ValidationDto<IList<ReservationDto>>> GetReservationsAsync(int clientId)
        {
            var client = await GetClientAndReservationsAsync(clientId);

            if (client == null)
            {
                return new ValidationDto<IList<ReservationDto>>("Client not found");
            }

            return new ValidationDto<IList<ReservationDto>>()
            {
                Value = _mapper.Map<IList<ReservationDto>>(client.ClientRooms)
            };
        }

        public async Task<ValidationDto<object>> CancelReservationAsync(int clientId, int reservationId)
        {
            var client = await GetClientAndReservationsAsync(clientId);

            if (client == null)
            {
                return new ValidationDto<object>("Client not found");
            }

            var clientRoom = client.ClientRooms.SingleOrDefault(s => s.Id == reservationId);

            if (clientRoom == null)
            {
                return new ValidationDto<object>("Reservation not found");
            }

            var command = new DeleteReservationCommand()
            {
                ClientId = clientId,
                ClientRoomId = reservationId
            };

            await _mediator.Send(command);

            return new ValidationDto<object>();
        }

        public async Task<ValidationDto<ReservationDto>> ModifyReservationAsync(int clientId, int reservationId, DateTime from, DateTime to)
        {
            var client = await GetClientAndReservationsAsync(clientId);

            if (client == null)
            {
                return new ValidationDto<ReservationDto>("Client not found");
            }

            var clientRoom = client.ClientRooms.SingleOrDefault(s => s.Id == reservationId);

            if (clientRoom == null)
            {
                return new ValidationDto<ReservationDto>("Reservation not found");
            }

            var reservations = _mapper.Map<IList<ReservationDto>>(client.ClientRooms);

            var currentReservation = _mapper.Map<ReservationDto>(clientRoom);

            var validation = ValidateReservationService.Validate(reservations, currentReservation);

            if (validation.IsValid)
            {
                var command = new ModifyReservationCommand()
                {
                    ClientId = clientId,
                    ClientRoomId = reservationId,
                    From = from,
                    To = to
                };

                await _mediator.Send(command);
            }

            return validation;
        }

        private async Task<Client> GetClientAsync(int clientId)
        {
            var query = new GetClientQuery()
            {
                ClientId = clientId
            };

            return await _mediator.Send(query);
        }

        private async Task<Client> GetClientAndReservationsAsync(int clientId)
        {
            var query = new GetClientAndReservationsQuery()
            {
                ClientId = clientId
            };

            return await _mediator.Send(query);
        }
    }
}