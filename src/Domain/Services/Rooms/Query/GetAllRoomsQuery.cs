using System.Collections.Generic;
using MediatR;
using HotelCancun.Domain.Entities;

namespace HotelCancun.Domain.Services.Rooms.Query 
{
    public class GetAllRoomsQuery : IRequest<IList<Room>> 
    { }
}