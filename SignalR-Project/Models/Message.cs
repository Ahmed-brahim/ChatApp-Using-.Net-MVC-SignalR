using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SignalR_Project.Models
{
    public class Message:ISoftDelete
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? SenderName { get; set; } // Name of the chat group
        public string ChatId { get; set; }
        [ForeignKey("ChatId")]
        public virtual Chat Chat { get; set; }

        public string SenderId { get; set; }
        [ForeignKey("SenderId")]
        public virtual ApplicationUser Sender { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public int StatusId { get; set; }
        public virtual MessageStatus Status { get; set; } // 1 = Sent, 2 = Delivered, 3 = Read
                                                          //soft delete properties
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public void Undo()
        {
            IsDeleted = false;
            DeletedAt = null;
        }

    }
    public enum MessageStatusEnum
    {
        Unread = 1,
        Delivered = 2,
        Seen = 3
    }
}