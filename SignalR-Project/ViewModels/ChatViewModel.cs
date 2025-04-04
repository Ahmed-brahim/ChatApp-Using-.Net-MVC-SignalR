using System;
using System.Collections.Generic;

namespace SignalR_Project.ViewModels
{
        public class ChatViewModel
        {
            public string Chat_Id { get; set; }
            public string Chat_Name { get; set; }
            public string Chat_Image { get; set; } 
            public string? LastMessage { get; set; }
            public DateTime? LastMessageTime { get; set; }
            public int UnreadCount { get; set; }
        }
}
