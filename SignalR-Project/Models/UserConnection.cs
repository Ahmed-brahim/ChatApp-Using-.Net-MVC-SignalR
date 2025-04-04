
using System.ComponentModel.DataAnnotations.Schema;

namespace SignalR_Project.Models
{
    public class UserConnection: ISoftDelete
    {
        public string ConnectionId { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        public void Undo()
        {
            IsDeleted = false;
            DeletedAt = null;
        }
    }
}
