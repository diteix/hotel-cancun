using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelCancun.Domain.Entities
{
    [Table("ClientsRooms")]
    public class ClientRoom
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime From { get; set; }
        [Required]
        public DateTime To { get; set; }
        [Required]
        [ForeignKey(nameof(Entities.Room))]
        public int RoomId { get; set; }
        public Room Room { get; set; }
        [Required]
        [ForeignKey(nameof(Entities.Client))]
        public int ClientId { get; set; }
        public Client Client { get; set; }
    }
}