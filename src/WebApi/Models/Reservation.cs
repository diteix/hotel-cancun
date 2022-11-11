using System.ComponentModel.DataAnnotations;

namespace HotelCancun.WebApi.Models;

public class Reservation
{
    [Required]
    [DataType(DataType.Date)]
    public DateTime From { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime To { get; set; }
}
