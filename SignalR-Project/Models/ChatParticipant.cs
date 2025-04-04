using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SignalR_Project.Models
{
    public class ChatParticipant: ISoftDelete
    {
        // Properties
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string ChatId { get; set; }
        [ForeignKey("ChatId")]
        public virtual Chat Chat { get; set; }

        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public bool IsAdmin { get; set; } = false;
        // Soft delete properties
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public void Undo()
        {
            IsDeleted = false;
            DeletedAt = null;
        }

    }
}
