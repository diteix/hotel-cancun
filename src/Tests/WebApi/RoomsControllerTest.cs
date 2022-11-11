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
using Moq;
using Xunit;

namespace HotelCancun.WebApi.Tests;

public class RoomsControllerTest
{
    private readonly ClientsController _controller;
    
    private readonly Mock<IClientApplication> _application;

    public RoomsControllerTest()
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
}