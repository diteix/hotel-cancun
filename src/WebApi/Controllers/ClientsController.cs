using Microsoft.AspNetCore.Mvc;
using HotelCancun.Application.Services.Interfaces;
using HotelCancun.Application.Dtos.Client;
using HotelCancun.Application.Dtos.Reservation;
using HotelCancun.WebApi.Models;
using HotelCancun.Application.Dtos.Validation;

namespace HotelCancun.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientApplication _application;

    public ClientsController(IClientApplication application)
    {
        _application = application;
    }

    [ProducesResponseType(typeof(ClientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationDto<ClientDto>), StatusCodes.Status404NotFound)]
    [HttpGet("{clientId:int}")]
    public async Task<IActionResult> GetClient(int clientId)
    {
        var validation = await _application.GetAsync(clientId);

        if (!validation.IsValid)
        {
            return NotFound(validation);
        }

        return Ok(validation.Value);
    }

    [ProducesResponseType(typeof(IList<ReservationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationDto<IList<ReservationDto>>), StatusCodes.Status404NotFound)]
    [HttpGet("{clientId:int}/reservations")]
    public async Task<IActionResult> GetReservations(int clientId)
    {
        var validation = await _application.GetReservationsAsync(clientId);

        if (!validation.IsValid)
        {
            return NotFound(validation);
        }

        return Ok(validation.Value);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationDto<object>), StatusCodes.Status404NotFound)]
    [HttpDelete("{clientId:int}/reservations/{reservationId:int}")]
    public async Task<IActionResult> CancelReservation([FromRoute] int clientId, [FromRoute] int reservationId)
    {
        var validation = await _application.CancelReservationAsync(clientId, reservationId);

        if (!validation.IsValid)
        {
            return NotFound(validation);
        }

        return NoContent();
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationDto<ReservationDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ValidationDto<ReservationDto>), StatusCodes.Status404NotFound)]
    [HttpPut("{clientId:int}/reservations/{reservationId:int}")]
    public async Task<IActionResult> ModifyReservation([FromRoute] int clientId, [FromRoute] int reservationId, [FromBody] Reservation reservation)
    {
        var validation = await _application.ModifyReservationAsync(clientId, reservationId, reservation.From, reservation.To);

        if (!validation.IsValid && validation.Value != null)
        {
            return BadRequest(validation);
        }
        else if (!validation.IsValid)
        {
            return NotFound(validation);
        }

        return NoContent();
    }
}
