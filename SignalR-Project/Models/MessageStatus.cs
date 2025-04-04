using System.ComponentModel.DataAnnotations;

namespace SignalR_Project.Models
{
    public class MessageStatus
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string StatusName { get; set; }

        // Navigation properties
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
