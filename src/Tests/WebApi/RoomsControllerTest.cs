using HotelCancun.Application.Dtos.Client;
using HotelCancun.Application.Dtos.Reservation;
using HotelCancun.Application.Dtos.Room;
using HotelCancun.Application.Dtos.Validation;
using HotelCancun.Application.Services.Clients;
using HotelCancun.Application.Services.Interfaces;
using HotelCancun.WebApi.Controllers;
using HotelCancun.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace HotelCancun.WebApi.Tests;

public class RoomsControllerTest
{
    private readonly RoomsController _controller;
    
    private readonly Mock<IRoomApplication> _application;
    private readonly Mock<IClientApplication> _clientApplication;

    public RoomsControllerTest()
    {
        _application = new Mock<IRoomApplication>();
        _clientApplication = new Mock<IClientApplication>();

        _controller = new RoomsController(_application.Object, _clientApplication.Object);
    }

    [Fact]
    public async Task GetRooms_ShouldReturnOk()
    {
        _application.Setup(s => s.GetAllRoomAsync()).ReturnsAsync(new List<RoomDto>());

        var result = await _controller.GetRooms();

        Assert.Equal((int)HttpStatusCode.OK, ((OkObjectResult)result.Result).StatusCode);
    }

    [Fact]
    public async Task GetRoom_ShouldReturnOk()
    {
        _application.Setup(s => s.GetRoomAndReservationsAsync(It.IsAny<int>())).ReturnsAsync(new ValidationDto<RoomDto>());

        var result = await _controller.GetRoom(It.IsAny<int>());

        Assert.Equal((int)HttpStatusCode.OK, ((OkObjectResult)result).StatusCode);
    }

    [Fact]
    public async Task GetRoom_ShouldReturnNotFound()
    {
        _application.Setup(s => s.GetRoomAndReservationsAsync(It.IsAny<int>())).ReturnsAsync(new ValidationDto<RoomDto>("Not Found"));

        var result = await _controller.GetRoom(It.IsAny<int>());

        Assert.Equal((int)HttpStatusCode.NotFound, ((NotFoundObjectResult)result).StatusCode);
    }

    [Fact]
    public async Task BookRoom_ShouldReturnCreated()
    {
        _clientApplication.Setup(s => s.GetAsync(It.IsAny<int>()))
            .ReturnsAsync(new ValidationDto<ClientDto>());
        _application.Setup(s => s.AddReservationAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new ValidationDto<ReservationDto>());
        var booking = new Booking();

        var result = await _controller.BookRoom(It.IsAny<int>(), booking);

        Assert.Equal((int)HttpStatusCode.Created, ((CreatedResult)result).StatusCode);
    }

    [Fact]
    public async Task BookRoom_ShouldReturnBadRequest()
    {
        _clientApplication.Setup(s => s.GetAsync(It.IsAny<int>()))
            .ReturnsAsync(new ValidationDto<ClientDto>());
        _application.Setup(s => s.AddReservationAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new ValidationDto<ReservationDto>("Bad Request") { Value = new ReservationDto() });
        var booking = new Booking();

        var result = await _controller.BookRoom(It.IsAny<int>(), booking);

        Assert.Equal((int)HttpStatusCode.BadRequest, ((BadRequestObjectResult)result).StatusCode);
    }

    [Fact]
    public async Task BookRoom_ShouldReturnClientNotFound()
    {
        var clientNotFoundMessage = "Client not Found";
        _clientApplication.Setup(s => s.GetAsync(It.IsAny<int>()))
            .ReturnsAsync(new ValidationDto<ClientDto>(clientNotFoundMessage));
        var booking = new Booking();

        var result = await _controller.BookRoom(It.IsAny<int>(), booking);

        Assert.Equal((int)HttpStatusCode.NotFound, ((NotFoundObjectResult)result).StatusCode);
        Assert.Equal(clientNotFoundMessage, ((ValidationDto<ClientDto>)((NotFoundObjectResult)result).Value).ValidationMessages.First());
    }

    [Fact]
    public async Task BookRoom_ShouldReturnRoomNotFound()
    {
        var roomNotFoundMessage = "Room not Found";
        _clientApplication.Setup(s => s.GetAsync(It.IsAny<int>()))
            .ReturnsAsync(new ValidationDto<ClientDto>());
        _application.Setup(s => s.AddReservationAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new ValidationDto<ReservationDto>(roomNotFoundMessage));
        var booking = new Booking();

        var result = await _controller.BookRoom(It.IsAny<int>(), booking);

        Assert.Equal((int)HttpStatusCode.NotFound, ((NotFoundObjectResult)result).StatusCode);
        Assert.Equal(roomNotFoundMessage, ((ValidationDto<ReservationDto>)((NotFoundObjectResult)result).Value).ValidationMessages.First());
    }
}