using System;
using System.Collections.Generic;

namespace SignalR_Project.ViewModels
{
    public class ChatDashboardViewModel
    {
        // User information
        public string CurrentUserId { get; set; }
        public string CurrentUserName { get; set; }
        public string CurrentUserEmail { get; set; }
        public string CurrentUserFirstName { get; set; }
        public string CurrentUserLastName { get; set; }

        // List of available chats/conversations
        public List<ChatConversation> Chats { get; set; }

        // Currently selected chat conversation
        public ChatConversation CurrentChat { get; set; }

        // New message input
        public string NewMessageText { get; set; }
    }

    public class ChatConversation
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime LastMessageTime { get; set; }
        public string LastMessage { get; set; }
        public int UnreadCount { get; set; }
        public List<ChatMessage> Messages { get; set; }
        public bool IsGroupChat { get; set; }
        public List<ChatParticipant> Participants { get; set; }
    }

    public class ChatMessage
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public DateTime TimeSent { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public bool IsSentByUser { get; set; } // Indicates if this message was sent by the current user
        public MessageStatus Status { get; set; }
    }

    public class ChatParticipant
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsOnline { get; set; }
    }

    public enum MessageStatus
    {
        Sent,
        Delivered,
        Read
    }
}