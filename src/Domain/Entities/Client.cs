using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelCancun.Domain.Entities
{
    [Table("Clients")]
    public class Client
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [ForeignKey(nameof(ClientRoom.ClientId))]
        public virtual ICollection<ClientRoom> ClientRooms { get; set; }
    }
}