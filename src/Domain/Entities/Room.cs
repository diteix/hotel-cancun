using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelCancun.Domain.Entities
{
    [Table("Rooms")]
    public class Room
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Number { get; set; }
        [ForeignKey(nameof(ClientRoom.RoomId))]
        public virtual ICollection<ClientRoom> ClientRooms { get; set; }
    }
}