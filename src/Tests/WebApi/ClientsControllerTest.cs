using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using HotelCancun.Application.Services.Interfaces;
using HotelCancun.WebApi.Controllers;
using HotelCancun.Application.Dtos.Validation;
using HotelCancun.Application.Dtos.Client;
using HotelCancun.Application.Dtos.Reservation;
using HotelCancun.WebApi.Models;
using Moq;
using Xunit;

namespace HotelCancun.WebApi.Tests;

public class ClientsControllerTest
{
    private readonly ClientsController _controller;
    
    private readonly Mock<IClientApplication> _application;

    public ClientsControllerTest()
    {
        _application = new Mock<IClientApplication>();

        _controller = new ClientsController(_application.Object);
    }

    [Fact]
    public async Task GetClient_ShouldReturnOk()
    {
        _application.Setup(s => s.GetAsync(It.IsAny<int>())).ReturnsAsync(new ValidationDto<ClientDto>());

        var result = await _controller.GetClient(It.IsAny<int>());

        Assert.Equal((int)HttpStatusCode.OK, (result as OkObjectResult).StatusCode);
    }

    [Fact]
    public async Task GetClient_ShouldReturnNotFound()
    {
        _application.Setup(s => s.GetAsync(It.IsAny<int>())).ReturnsAsync(new ValidationDto<ClientDto>("Not Found"));

        var result = await _controller.GetClient(It.IsAny<int>());

        Assert.Equal((int)HttpStatusCode.NotFound, (result as NotFoundObjectResult).StatusCode);
    }

    [Fact]
    public async Task GetReservations_ShouldReturnOk()
    {
        _application.Setup(s => s.GetReservationsAsync(It.IsAny<int>())).ReturnsAsync(new ValidationDto<IList<ReservationDto>>());

        var result = await _controller.GetReservations(It.IsAny<int>());

        Assert.Equal((int)HttpStatusCode.OK, (result as OkObjectResult).StatusCode);
    }

    [Fact]
    public async Task GetReservations_ShouldReturnNotFound()
    {
        _application.Setup(s => s.GetReservationsAsync(It.IsAny<int>())).ReturnsAsync(new ValidationDto<IList<ReservationDto>>("Not Found"));

        var result = await _controller.GetReservations(It.IsAny<int>());

        Assert.Equal((int)HttpStatusCode.NotFound, (result as NotFoundObjectResult).StatusCode);
    }

    [Fact]
    public async Task CancelReservation_ShouldReturnNoContent()
    {
        _application.Setup(s => s.CancelReservationAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new ValidationDto<object>());

        var result = await _controller.CancelReservation(It.IsAny<int>(), It.IsAny<int>());

        Assert.Equal((int)HttpStatusCode.NoContent, (result as NoContentResult).StatusCode);
    }

    [Fact]
    public async Task CancelReservation_ShouldReturnNotFound()
    {
        _application.Setup(s => s.CancelReservationAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new ValidationDto<object>("Not Found"));

        var result = await _controller.CancelReservation(It.IsAny<int>(), It.IsAny<int>());

        Assert.Equal((int)HttpStatusCode.NotFound, (result as NotFoundObjectResult).StatusCode);
    }

    [Fact]
    public async Task ModifyReservation_ShouldReturnNoContent()
    {
        _application.Setup(s => s.ModifyReservationAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new ValidationDto<ReservationDto>());

        var result = await _controller.ModifyReservation(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Reservation>());

        Assert.Equal((int)HttpStatusCode.NoContent, (result as NoContentResult).StatusCode);
    }

    [Fact]
    public async Task ModifyReservation_ShouldReturnBadRequest()
    {
        _application.Setup(s => s.ModifyReservationAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new ValidationDto<ReservationDto>("Bad Request"){ Value = new ReservationDto() });

        var result = await _controller.ModifyReservation(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Reservation>());

        Assert.Equal((int)HttpStatusCode.BadRequest, (result as BadRequestObjectResult).StatusCode);
    }

    [Fact]
    public async Task ModifyReservation_ShouldReturnNotFound()
    {
        _application.Setup(s => s.ModifyReservationAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new ValidationDto<ReservationDto>("Not Found"));

        var result = await _controller.ModifyReservation(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Reservation>());

        Assert.Equal((int)HttpStatusCode.NotFound, (result as NotFoundObjectResult).StatusCode);
    }
}