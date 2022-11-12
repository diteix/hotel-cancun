using AutoMapper;
using HotelCancun.Application.Dtos.Client;
using HotelCancun.Application.Dtos.Reservation;
using HotelCancun.Application.Dtos.Room;
using HotelCancun.Application.Services.Clients;
using HotelCancun.Application.Services.Interfaces;
using HotelCancun.Application.Services.Rooms;
using HotelCancun.Domain.Entities;
using HotelCancun.Domain.Services.Rooms.Command;
using HotelCancun.Domain.Services.Rooms.Query;
using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HotelCancun.Application.Tests;

public class RoomApplicationTest
{
    private readonly IRoomApplication _application;
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<IMapper> _mapper;

    public RoomApplicationTest()
    {
        _mediator = new Mock<IMediator>();
        _mapper = new Mock<IMapper>();

        _mapper.Setup(s => s.Map<RoomDto>(It.IsAny<Room>())).Returns(new RoomDto());
        _mapper.Setup(s => s.Map<IList<RoomDto>>(It.IsAny<ICollection<Room>>())).Returns(new List<RoomDto>());

        _mapper.Setup(s => s.Map<ReservationDto>(It.IsAny<ClientRoom>())).Returns(new ReservationDto());
        _mapper.Setup(s => s.Map<IList<ReservationDto>>(It.IsAny<ICollection<ClientRoom>>())).Returns(new List<ReservationDto>());

        _application = new RoomApplication(_mediator.Object, _mapper.Object);
    }

    [Fact]
    public async Task GetAllRoomAsync_ShouldReturnRoomDtoList()
    {
        _mediator.Setup(s => s.Send(It.IsAny<GetAllRoomsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Room>());

        var result = await _application.GetAllRoomAsync();

        Assert.IsAssignableFrom<IList<RoomDto>>(result);
        _mediator.Verify(s =>
            s.Send(It.IsAny<GetAllRoomsQuery>(), It.IsAny<CancellationToken>()), Times.Once
        );
    }

    [Fact]
    public async Task GetRoomAndReservationsAsync_ShouldReturnValidValidationRoomDto()
    {
        _mediator.Setup(s => s.Send(It.IsAny<GetRoomReservationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Room());
        var id = It.IsAny<int>();

        var result = await _application.GetRoomAndReservationsAsync(id);

        Assert.IsType<RoomDto>(result.Value);
        Assert.True(result.IsValid);
        _mediator.Verify(s =>
            s.Send(It.Is<GetRoomReservationsQuery>(c => c.RoomId == id), It.IsAny<CancellationToken>())
        );
    }

    [Fact]
    public async Task GetRoomAndReservationsAsync_ShouldReturnInvalidValidationRoomDto()
    {
        _mediator.Setup(s => s.Send(It.IsAny<GetRoomReservationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Room);
        var id = It.IsAny<int>();

        var result = await _application.GetRoomAndReservationsAsync(id);

        Assert.Null(result.Value);
        Assert.False(result.IsValid);
        _mediator.Verify(s =>
            s.Send(It.Is<GetRoomReservationsQuery>(c => c.RoomId == id), It.IsAny<CancellationToken>())
        );
    }

    [Fact]
    public async Task AddReservationAsync_ShouldReturnValidValidationReservationDto()
    {
        var reservationFrom = DateTime.Today.AddDays(1);
        var reservationTo = DateTime.Today.AddDays(1);
        var roomId = It.IsAny<int>();
        var clientId = It.IsAny<int>();
        var reservations = new List<ReservationDto>();
        _mediator.Setup(s => s.Send(It.IsAny<GetRoomReservationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Room() { ClientRooms = new List<ClientRoom>() });
        _mediator.Setup(s => s.Send(It.IsAny<AddReservationCommand>(), It.IsAny<CancellationToken>()));

        _mapper.Setup(s => s.Map<IList<ReservationDto>>(It.IsAny<ICollection<ClientRoom>>())).Returns(reservations);

        var result = await _application.AddReservationAsync(roomId, clientId, reservationFrom, reservationTo);

        Assert.Null(result.Value);
        Assert.True(result.IsValid);
        _mediator.Verify(s =>
            s.Send(It.Is<GetRoomReservationsQuery>(c => c.RoomId == roomId), It.IsAny<CancellationToken>())
        );
        _mediator.Verify(s =>
            s.Send(It.Is<AddReservationCommand>(c => c.RoomId == roomId && c.ClientId == clientId), It.IsAny<CancellationToken>())
        );
    }

    [Fact]
    public async Task AddReservationAsync_ShouldReturnInvalidValidationReservationDtoWhenRoomNotFound()
    {
        var roomId = It.IsAny<int>();
        var clientId = It.IsAny<int>();
        _mediator.Setup(s => s.Send(It.IsAny<GetRoomReservationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Room);

        var result = await _application.AddReservationAsync(roomId, clientId, It.IsAny<DateTime>(), It.IsAny<DateTime>());

        Assert.Null(result.Value);
        Assert.False(result.IsValid);
        _mediator.Verify(s =>
            s.Send(It.Is<GetRoomReservationsQuery>(c => c.RoomId == roomId), It.IsAny<CancellationToken>())
        );
    }

    [Fact]
    public async Task AddReservationAsync_ShouldReturnInvalidValidationReservationDto_StartCantBeLaterThanEnd()
    {
        var reservationFrom = DateTime.Today.AddDays(4);
        var reservationTo = DateTime.Today.AddDays(2);
        var roomId = It.IsAny<int>();
        var clientId = It.IsAny<int>();
        var reservations = new List<ReservationDto>();
        _mediator.Setup(s => s.Send(It.IsAny<GetRoomReservationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Room() { ClientRooms = new List<ClientRoom>() });

        _mapper.Setup(s => s.Map<IList<ReservationDto>>(It.IsAny<ICollection<ClientRoom>>())).Returns(reservations);

        var result = await _application.AddReservationAsync(roomId, clientId, reservationFrom, reservationTo);

        Assert.IsType<ReservationDto>(result.Value);
        Assert.False(result.IsValid);
        Assert.Equal("Start date of reservation can't later than the end date.", result.ValidationMessages.First());
        _mediator.Verify(s =>
            s.Send(It.Is<GetRoomReservationsQuery>(c => c.RoomId == roomId), It.IsAny<CancellationToken>())
        );
    }

    [Fact]
    public async Task AddReservationAsync_ShouldReturnInvalidValidationReservationDto_CantStayLongerThan3Days()
    {
        var reservationFrom = DateTime.Today.AddDays(1);
        var reservationTo = DateTime.Today.AddDays(4);
        var roomId = It.IsAny<int>();
        var clientId = It.IsAny<int>();
        var reservations = new List<ReservationDto>();
        _mediator.Setup(s => s.Send(It.IsAny<GetRoomReservationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Room() { ClientRooms = new List<ClientRoom>() });

        _mapper.Setup(s => s.Map<IList<ReservationDto>>(It.IsAny<ICollection<ClientRoom>>())).Returns(reservations);

        var result = await _application.AddReservationAsync(roomId, clientId, reservationFrom, reservationTo);

        Assert.IsType<ReservationDto>(result.Value);
        Assert.False(result.IsValid);
        Assert.Equal("Reservation can't be longer than 3 days.", result.ValidationMessages.First());
        _mediator.Verify(s =>
            s.Send(It.Is<GetRoomReservationsQuery>(c => c.RoomId == roomId), It.IsAny<CancellationToken>())
        );
    }

    [Fact]
    public async Task AddReservationAsync_ShouldReturnInvalidValidationReservationDto_CantBeReservedMoreThan30DaysInAdvance()
    {
        var reservationFrom = DateTime.Today.AddDays(31);
        var reservationTo = DateTime.Today.AddDays(31);
        var roomId = It.IsAny<int>();
        var clientId = It.IsAny<int>();
        var reservations = new List<ReservationDto>();
        _mediator.Setup(s => s.Send(It.IsAny<GetRoomReservationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Room() { ClientRooms = new List<ClientRoom>() });

        _mapper.Setup(s => s.Map<IList<ReservationDto>>(It.IsAny<ICollection<ClientRoom>>())).Returns(reservations);

        var result = await _application.AddReservationAsync(roomId, clientId, reservationFrom, reservationTo);

        Assert.IsType<ReservationDto>(result.Value);
        Assert.False(result.IsValid);
        Assert.Equal("Can't be reserved more than 30 days in advance.", result.ValidationMessages.First());
        _mediator.Verify(s =>
            s.Send(It.Is<GetRoomReservationsQuery>(c => c.RoomId == roomId), It.IsAny<CancellationToken>())
        );
    }

    [Fact]
    public async Task AddReservationAsync_ShouldReturnInvalidValidationReservationDto_ReservationShouldStartNextDayOfBooking()
    {
        var reservationFrom = DateTime.Today;
        var reservationTo = DateTime.Today;
        var roomId = It.IsAny<int>();
        var clientId = It.IsAny<int>();
        var reservations = new List<ReservationDto>();
        _mediator.Setup(s => s.Send(It.IsAny<GetRoomReservationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Room() { ClientRooms = new List<ClientRoom>() });

        _mapper.Setup(s => s.Map<IList<ReservationDto>>(It.IsAny<ICollection<ClientRoom>>())).Returns(reservations);

        var result = await _application.AddReservationAsync(roomId, clientId, reservationFrom, reservationTo);

        Assert.IsType<ReservationDto>(result.Value);
        Assert.False(result.IsValid);
        Assert.Equal("All reservations start at least the next day of booking.", result.ValidationMessages.First());
        _mediator.Verify(s =>
            s.Send(It.Is<GetRoomReservationsQuery>(c => c.RoomId == roomId), It.IsAny<CancellationToken>())
        );
    }

    [Theory]
    [MemberData(nameof(GetReservationDates))]
    public async Task AddReservationAsync_ShouldReturnInvalidValidationReservationDto_RoomAlreadyReserved(
        DateTime from,
        DateTime to
    )
    {
        var reservationFrom = from;
        var reservationTo = to;
        var roomId = It.IsAny<int>();
        var clientId = It.IsAny<int>();
        var reservations = new List<ReservationDto>()
        {
            new ReservationDto()
            {
                Id = 1,
                From = DateTime.Today.AddDays(5),
                To = DateTime.Today.AddDays(7).Date.AddHours(23).AddMinutes(59).AddSeconds(59)
            }
        };
        _mediator.Setup(s => s.Send(It.IsAny<GetRoomReservationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Room() { ClientRooms = new List<ClientRoom>() });

        _mapper.Setup(s => s.Map<IList<ReservationDto>>(It.IsAny<ICollection<ClientRoom>>())).Returns(reservations);

        var result = await _application.AddReservationAsync(roomId, clientId, reservationFrom, reservationTo);

        Assert.IsType<ReservationDto>(result.Value);
        Assert.False(result.IsValid);
        Assert.Equal("Room already reserverd in the requested dates.", result.ValidationMessages.First());
        _mediator.Verify(s =>
            s.Send(It.Is<GetRoomReservationsQuery>(c => c.RoomId == roomId), It.IsAny<CancellationToken>())
        );
    }

    public static IEnumerable<object[]> GetReservationDates()
    {
        yield return new object[] { DateTime.Today.AddDays(3), DateTime.Today.AddDays(5) };
        yield return new object[] { DateTime.Today.AddDays(7), DateTime.Today.AddDays(9) };
    }
}
