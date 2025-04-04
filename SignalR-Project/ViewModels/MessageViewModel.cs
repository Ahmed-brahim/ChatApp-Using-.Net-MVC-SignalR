using System;

namespace SignalR_Project.ViewModels
{
    public class MessageViewModel
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string SenderName { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsSentByUser { get; set; }
        public string Status { get; set; }
    }
}