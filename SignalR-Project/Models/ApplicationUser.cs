using Microsoft.AspNetCore.Identity;
using SignalR_Project.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace SignalR_Project.Models
{
    public class ApplicationUser : IdentityUser , ISoftDelete
    {
        [Required(ErrorMessage = "First Name is required")]
        [MinLength(3,ErrorMessage = "At least 3 Characters")]
        public String FirstName { get; set; }
        public String? LastName { get; set; }
        public String? Country { get; set; }
        public  string image { get; set;}
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        void ISoftDelete.Undo()
        {    
            IsDeleted = false;
            DeletedAt = null;
        }


        //*************
        // Navigation properties
        public  ICollection<UserConnection> Connections { get; set; } = new List<UserConnection>();
        public  ICollection<ChatParticipant> ParticipatingChats { get; set; } = new List<ChatParticipant>();
        public ICollection<Message> SentMessages { get; set; } = new List<Message>();

    }
}
