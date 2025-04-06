using System;
using System.Collections.Generic;

namespace SignalR_Project.ViewModels
{
    public class ChatViewModel
    {
        public string Chat_Id { get; set; }
        public bool IsGroupChat { get; set; }
        public string Chat_Name { get; set; }
        public string Chat_Image { get; set; }
        public List<MessageViewModel2> Messages { get; set; }
        public List<ParticipantViewModel> Participants { get; set; }

        public ChatViewModel()
        {
            Messages = new List<MessageViewModel2>();
            Participants = new List<ParticipantViewModel>();
        }
    }

    // ViewModel for representing a message
    public class MessageViewModel2
    {
        public string SenderName { get; set; }
        public string Id { get; set; } // Message ID
        public string SenderId { get; set; } // Could be a user identifier (e.g., email or ID)
        public string Text { get; set; }
        public DateTime TimeSent { get; set; }
    }

    // ViewModel for representing a participant in a chat group
    public class ParticipantViewModel
    {
        public string ParticipantName { get; set; }
        public string ParticipantId { get; set; } // Could be a user identifier
        public string ParticipantImage { get; set; } // URL to the participant's profile picture
    }
}
