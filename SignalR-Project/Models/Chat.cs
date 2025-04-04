using SignalR_Project.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace SignalR_Project.Models
{
    public class Chat:ISoftDelete
    {
        // Properties
    
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Name { get; set; }

        public bool IsGroupChat { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<ChatParticipant> Participants { get; set; } = new List<ChatParticipant>();
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
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
