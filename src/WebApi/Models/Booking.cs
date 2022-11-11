using System.ComponentModel.DataAnnotations;

namespace HotelCancun.WebApi.Models;

public class Booking
{
    [Required]
    [DataType(DataType.Date)]
    public DateTime From { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime To { get; set; }

    [Required]
    public int ClientId { get; set; }
}
