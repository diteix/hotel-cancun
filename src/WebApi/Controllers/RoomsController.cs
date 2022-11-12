using Microsoft.AspNetCore.Mvc;
using HotelCancun.WebApi.Models;
using HotelCancun.Application.Services.Interfaces;
using HotelCancun.Application.Dtos.Room;
using HotelCancun.Application.Dtos.Reservation;
using HotelCancun.Application.Dtos.Validation;
using HotelCancun.Application.Dtos.Client;

namespace HotelCancun.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IRoomApplication _application;
    private readonly IClientApplication _clientApplication;

    public RoomsController(IRoomApplication application, IClientApplication clientApplication)
    {
        _application = application;
        _clientApplication = clientApplication;
    }

    [ProducesResponseType(typeof(IList<RoomDto>), StatusCodes.Status200OK)]
    [HttpGet()]
    public async Task<ActionResult<IList<RoomDto>>> GetRooms()
    {
        return Ok(await _application.GetAllRoomAsync());
    }

    [ProducesResponseType(typeof(RoomDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationDto<RoomDto>), StatusCodes.Status404NotFound)]
    [HttpGet("{roomId:int}")]
    public async Task<IActionResult> GetRoom(int roomId)
    {
        var validation = await _application.GetRoomAndReservationsAsync(roomId);

        if (!validation.IsValid)
        {
            return NotFound(validation);
        }

        return Ok(validation.Value);
    }

    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationDto<ReservationDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ValidationDto<ClientDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationDto<ReservationDto>), StatusCodes.Status404NotFound)]
    [HttpPost("{roomId:int}/reservation")]
    public async Task<IActionResult> BookRoom([FromRoute] int roomId, [FromBody] Booking booking)
    {
        var clientValidation = await _clientApplication.GetAsync(booking.ClientId);

        if (!clientValidation.IsValid)
        {
            return NotFound(clientValidation);
        }

        var validation = await _application.AddReservationAsync(
            roomId, 
            booking.ClientId, 
            booking.From, 
            booking.To
        );

        if (!validation.IsValid && validation.Value != null)
        {
            return BadRequest(validation);
        }
        else if (!validation.IsValid)
        {
            return NotFound(validation);
        }

        return Created($"api/Clients/{booking.ClientId}/reservations", booking);
    }
}
