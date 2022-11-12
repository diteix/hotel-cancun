using AutoMapper;
using HotelCancun.Application.Dtos.Client;
using HotelCancun.Application.Dtos.Reservation;
using HotelCancun.Application.Dtos.Validation;
using HotelCancun.Application.Services.Clients;
using HotelCancun.Application.Services.Interfaces;
using HotelCancun.Domain.Entities;
using HotelCancun.Domain.Services.Clients.Command;
using HotelCancun.Domain.Services.Clients.Query;
using MediatR;
using Moq;
using Xunit;

namespace HotelCancun.Application.Tests;

public class ClientApplicationTest
{
    private readonly IClientApplication _application;
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<IMapper> _mapper;

    public ClientApplicationTest()
    {
        _mediator = new Mock<IMediator>();
        _mapper = new Mock<IMapper>();

        _mapper.Setup(s => s.Map<ClientDto>(It.IsAny<Client>())).Returns(new ClientDto());

        _mapper.Setup(s => s.Map<ReservationDto>(It.IsAny<ClientRoom>())).Returns(new ReservationDto());
        _mapper.Setup(s => s.Map<IList<ReservationDto>>(It.IsAny<ICollection<ClientRoom>>())).Returns(new List<ReservationDto>());

        _application = new ClientApplication(_mediator.Object, _mapper.Object);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnValidValidationClientDto()
    {
        _mediator.Setup(s => s.Send(It.IsAny<GetClientQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Client());
        var id = It.IsAny<int>();

        var result = await _application.GetAsync(id);

        Assert.IsType<ClientDto>(result.Value);
        Assert.True(result.IsValid);
        _mediator.Verify(s =>
            s.Send(It.Is<GetClientQuery>(c => c.ClientId == id), It.IsAny<CancellationToken>())
        );
    }

    [Fact]
    public async Task GetAsync_ShouldReturnInvalidValidationClientDto()
    {
        _mediator.Setup(s => s.Send(It.IsAny<GetClientQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Client);
        var id = It.IsAny<int>();

        var result = await _application.GetAsync(id);

        Assert.Null(result.Value);
        Assert.False(result.IsValid);
        _mediator.Verify(s =>
            s.Send(It.Is<GetClientQuery>(c => c.ClientId == id), It.IsAny<CancellationToken>())
        );
    }

    [Fact]
    public async Task GetReservationsAsync_ShouldReturnValidValidationIListReservationDto()
    {
        _mediator.Setup(s => s.Send(It.IsAny<GetClientAndReservationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Client() { ClientRooms = new List<ClientRoom>() });
        var id = It.IsAny<int>();

        var result = await _application.GetReservationsAsync(id);

        Assert.IsAssignableFrom<IList<ReservationDto>>(result.Value);
        Assert.True(result.IsValid);
        _mediator.Verify(s =>
            s.Send(It.Is<GetClientAndReservationsQuery>(c => c.ClientId == id), It.IsAny<CancellationToken>())
        );
    }

    [Fact]
    public async Task GetReservationsAsync_ShouldReturnInvalidValidationIListReservationDto()
    {
        _mediator.Setup(s => s.Send(It.IsAny<GetClientAndReservationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Client);
        var id = It.IsAny<int>();

        var result = await _application.GetReservationsAsync(id);

        Assert.Null(result.Value);
        Assert.False(result.IsValid);
        _mediator.Verify(s =>
            s.Send(It.Is<GetClientAndReservationsQuery>(c => c.ClientId == id), It.IsAny<CancellationToken>())
        );
    }

    [Fact]
    public async Task CancelReservationAsync_ShouldReturnValidValidationObject()
    {
        var clientId = It.IsAny<int>();
        var reservationId = It.IsAny<int>();
        _mediator.Setup(s => s.Send(It.IsAny<GetClientAndReservationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Client() { ClientRooms = new List<ClientRoom>() { new ClientRoom() { ClientId = clientId, Id = reservationId} } });
        _mediator.Setup(s => s.Send(It.IsAny<DeleteReservationCommand>(), It.IsAny<CancellationToken>()));

        var result = await _application.CancelReservationAsync(clientId, reservationId);

        Assert.True(result.IsValid);
        _mediator.Verify(s =>
            s.Send(It.Is<GetClientAndReservationsQuery>(c => c.ClientId == clientId), It.IsAny<CancellationToken>())
        );
    }

    [Fact]
    public async Task CancelReservationAsync_ShouldReturnInvalidValidationObjectWhenClientNotFound()
    {
        var clientId = It.IsAny<int>();
        var reservationId = It.IsAny<int>();
        _mediator.Setup(s => s.Send(It.IsAny<GetClientAndReservationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Client);

        var result = await _application.CancelReservationAsync(clientId, reservationId);

        Assert.False(result.IsValid);
        _mediator.Verify(s =>
            s.Send(It.Is<GetClientAndReservationsQuery>(c => c.ClientId == clientId), It.IsAny<CancellationToken>())
        );
    }

    [Fact]
    public async Task CancelReservationAsync_ShouldReturnInvalidValidationObjectWhenReservationNotFound()
    {
        var clientId = It.IsAny<int>();
        var reservationId = It.IsAny<int>();
        _mediator.Setup(s => s.Send(It.IsAny<GetClientAndReservationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Client() { ClientRooms = new List<ClientRoom>() });

        var result = await _application.CancelReservationAsync(clientId, reservationId);

        Assert.False(result.IsValid);
        _mediator.Verify(s =>
            s.Send(It.Is<GetClientAndReservationsQuery>(c => c.ClientId == clientId), It.IsAny<CancellationToken>())
        );
    }

    [Fact]
    public async Task ModifyReservationAsync_ShouldReturnValidValidationReservationDto()
    {
        var reservationFrom = DateTime.Today.AddDays(1);
        var reservationTo = DateTime.Today.AddDays(1);
        var clientId = It.IsAny<int>();
        var reservationId = It.IsAny<int>();
        var reservations = new List<ReservationDto>()
        {
            new ReservationDto()
            {
                Id = reservationId
            }
        };
        _mediator.Setup(s => s.Send(It.IsAny<GetClientAndReservationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Client() { ClientRooms = new List<ClientRoom>() { new ClientRoom() { Id = reservationId } } });
        _mediator.Setup(s => s.Send(It.IsAny<ModifyReservationCommand>(), It.IsAny<CancellationToken>()));

        _mapper.Setup(s => s.Map<IList<ReservationDto>>(It.IsAny<ICollection<ClientRoom>>())).Returns(reservations);

        var result = await _application.ModifyReservationAsync(clientId, reservationId, reservationFrom, reservationTo);

        Assert.Null(result.Value);
        Assert.True(result.IsValid);
        _mediator.Verify(s =>
            s.Send(It.Is<GetClientAndReservationsQuery>(c => c.ClientId == clientId), It.IsAny<CancellationToken>())
        );
        _mediator.Verify(s =>
            s.Send(It.Is<ModifyReservationCommand>(c => c.ClientId == clientId && c.ClientRoomId == reservationId), It.IsAny<CancellationToken>())
        );
    }

    [Fact]
    public async Task ModifyReservationAsync_ShouldReturnInvalidValidationReservationDtoWhenClientNotFound()
    {
        var clientId = It.IsAny<int>();
        var reservationId = It.IsAny<int>();
        _mediator.Setup(s => s.Send(It.IsAny<GetClientAndReservationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Client);

        var result = await _application.ModifyReservationAsync(clientId, reservationId, It.IsAny<DateTime>(), It.IsAny<DateTime>());

        Assert.Null(result.Value);
        Assert.False(result.IsValid);
        _mediator.Verify(s =>
            s.Send(It.Is<GetClientAndReservationsQuery>(c => c.ClientId == clientId), It.IsAny<CancellationToken>())
        );
    }

    [Fact]
    public async Task ModifyReservationAsync_ShouldReturnInvalidValidationReservationDtoWhenReservationNotFound()
    {
        var clientId = It.IsAny<int>();
        var reservationId = It.IsAny<int>();
        _mediator.Setup(s => s.Send(It.IsAny<GetClientAndReservationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Client() { ClientRooms = new List<ClientRoom>() });

        var result = await _application.ModifyReservationAsync(clientId, reservationId, It.IsAny<DateTime>(), It.IsAny<DateTime>());

        Assert.Null(result.Value);
        Assert.False(result.IsValid);
        _mediator.Verify(s =>
            s.Send(It.Is<GetClientAndReservationsQuery>(c => c.ClientId == clientId), It.IsAny<CancellationToken>())
        );
    }

    [Fact]
    public async Task ModifyReservationAsync_ShouldReturnInvalidValidationReservationDto_StartCantBeLaterThanEnd()
    {
        var reservationFrom = DateTime.Today.AddDays(4);
        var reservationTo = DateTime.Today.AddDays(2);
        var clientId = It.IsAny<int>();
        var reservationId = It.IsAny<int>();
        var reservations = new List<ReservationDto>();
        _mediator.Setup(s => s.Send(It.IsAny<GetClientAndReservationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Client() { ClientRooms = new List<ClientRoom>() { new ClientRoom() { Id = reservationId } } });

        _mapper.Setup(s => s.Map<IList<ReservationDto>>(It.IsAny<ICollection<ClientRoom>>())).Returns(reservations);

        var result = await _application.ModifyReservationAsync(clientId, reservationId, reservationFrom, reservationTo);

        Assert.IsType<ReservationDto>(result.Value);
        Assert.False(result.IsValid);
        Assert.Equal("Start date of reservation can't later than the end date.", result.ValidationMessages.First());
        _mediator.Verify(s =>
            s.Send(It.Is<GetClientAndReservationsQuery>(c => c.ClientId == clientId), It.IsAny<CancellationToken>())
        );
    }

    [Fact]
    public async Task ModifyReservationAsync_ShouldReturnInvalidValidationReservationDto_CantStayLongerThan3Days()
    {
        var reservationFrom = DateTime.Today.AddDays(1);
        var reservationTo = DateTime.Today.AddDays(4);
        var clientId = It.IsAny<int>();
        var reservationId = It.IsAny<int>();
        var reservations = new List<ReservationDto>();
        _mediator.Setup(s => s.Send(It.IsAny<GetClientAndReservationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Client() { ClientRooms = new List<ClientRoom>() { new ClientRoom() { Id = reservationId } } });

        _mapper.Setup(s => s.Map<IList<ReservationDto>>(It.IsAny<ICollection<ClientRoom>>())).Returns(reservations);

        var result = await _application.ModifyReservationAsync(clientId, reservationId, reservationFrom, reservationTo);

        Assert.IsType<ReservationDto>(result.Value);
        Assert.False(result.IsValid);
        Assert.Equal("Reservation can't be longer than 3 days.", result.ValidationMessages.First());
        _mediator.Verify(s =>
            s.Send(It.Is<GetClientAndReservationsQuery>(c => c.ClientId == clientId), It.IsAny<CancellationToken>())
        );
    }

    [Fact]
    public async Task ModifyReservationAsync_ShouldReturnInvalidValidationReservationDto_CantBeReservedMoreThan30DaysInAdvance()
    {
        var reservationFrom = DateTime.Today.AddDays(31);
        var reservationTo = DateTime.Today.AddDays(31);
        var clientId = It.IsAny<int>();
        var reservationId = It.IsAny<int>();
        var reservations = new List<ReservationDto>();
        _mediator.Setup(s => s.Send(It.IsAny<GetClientAndReservationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Client() { ClientRooms = new List<ClientRoom>() { new ClientRoom() { Id = reservationId } } });

        _mapper.Setup(s => s.Map<IList<ReservationDto>>(It.IsAny<ICollection<ClientRoom>>())).Returns(reservations);

        var result = await _application.ModifyReservationAsync(clientId, reservationId, reservationFrom, reservationTo);

        Assert.IsType<ReservationDto>(result.Value);
        Assert.False(result.IsValid);
        Assert.Equal("Can't be reserved more than 30 days in advance.", result.ValidationMessages.First());
        _mediator.Verify(s =>
            s.Send(It.Is<GetClientAndReservationsQuery>(c => c.ClientId == clientId), It.IsAny<CancellationToken>())
        );
    }

    [Fact]
    public async Task ModifyReservationAsync_ShouldReturnInvalidValidationReservationDto_ReservationShouldStartNextDayOfBooking()
    {
        var reservationFrom = DateTime.Today;
        var reservationTo = DateTime.Today;
        var clientId = It.IsAny<int>();
        var reservationId = It.IsAny<int>();
        var reservations = new List<ReservationDto>();
        _mediator.Setup(s => s.Send(It.IsAny<GetClientAndReservationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Client() { ClientRooms = new List<ClientRoom>() { new ClientRoom() { Id = reservationId } } });

        _mapper.Setup(s => s.Map<IList<ReservationDto>>(It.IsAny<ICollection<ClientRoom>>())).Returns(reservations);

        var result = await _application.ModifyReservationAsync(clientId, reservationId, reservationFrom, reservationTo);

        Assert.IsType<ReservationDto>(result.Value);
        Assert.False(result.IsValid);
        Assert.Equal("All reservations start at least the next day of booking.", result.ValidationMessages.First());
        _mediator.Verify(s =>
            s.Send(It.Is<GetClientAndReservationsQuery>(c => c.ClientId == clientId), It.IsAny<CancellationToken>())
        );
    }

    [Theory]
    [MemberData(nameof(GetReservationDates))]
    public async Task ModifyReservationAsync_ShouldReturnInvalidValidationReservationDto_RoomAlreadyReserved(
        DateTime from,
        DateTime to
    )
    {
        var reservationFrom = from;
        var reservationTo = to;
        var clientId = It.IsAny<int>();
        var reservationId = It.IsAny<int>();
        var reservations = new List<ReservationDto>()
        {
            new ReservationDto()
            {
                Id = 1,
                From = DateTime.Today.AddDays(5),
                To = DateTime.Today.AddDays(7).Date.AddHours(23).AddMinutes(59).AddSeconds(59)
            },
            new ReservationDto()
            {
                Id = reservationId,
                From = DateTime.Today.AddDays(1),
                To = DateTime.Today.AddDays(1).Date.AddHours(23).AddMinutes(59).AddSeconds(59)
            }
        };
        _mediator.Setup(s => s.Send(It.IsAny<GetClientAndReservationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Client() { ClientRooms = new List<ClientRoom>() { new ClientRoom() { Id = reservationId } } });

        _mapper.Setup(s => s.Map<IList<ReservationDto>>(It.IsAny<ICollection<ClientRoom>>())).Returns(reservations);

        var result = await _application.ModifyReservationAsync(clientId, reservationId, reservationFrom, reservationTo);

        Assert.IsType<ReservationDto>(result.Value);
        Assert.False(result.IsValid);
        Assert.Equal("Room already reserverd in the requested dates.", result.ValidationMessages.First());
        _mediator.Verify(s =>
            s.Send(It.Is<GetClientAndReservationsQuery>(c => c.ClientId == clientId), It.IsAny<CancellationToken>())
        );
    }

    public static IEnumerable<object[]> GetReservationDates()
    {
        yield return new object[] { DateTime.Today.AddDays(3), DateTime.Today.AddDays(5) };
        yield return new object[] { DateTime.Today.AddDays(7), DateTime.Today.AddDays(9) };
    }
}