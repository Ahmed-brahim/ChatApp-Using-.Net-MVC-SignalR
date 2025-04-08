using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using SignalR_Project.Models;
using SignalR_Project.Models.Data;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace SignalR_Project.Hubs
{
	public class ChatHub : Hub
	{
		private readonly AppDbContext _context;
        public ChatHub(AppDbContext context)
        {
            _context = context;
        }
        public async Task JoinChatGroup(string chatid)
        {
            var UserId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var messages = _context.Messages
                .Where(m => (m.ChatId == chatid) && (m.SenderId!= UserId))
                .ToList();
            foreach (var message in messages)
            {
                message.StatusId = (int)MessageStatusEnum.Seen;
            }
            _context.SaveChanges();
            if (_context.ChatParticipants.Any(p => (p.UserId == UserId) && (chatid == p.ChatId)))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, chatid);
            }
                
        }
        public async Task SendMessage(string sender_id, string message, string chat_id)
		{
            if (string.IsNullOrWhiteSpace(sender_id) || string.IsNullOrWhiteSpace(message) || string.IsNullOrWhiteSpace(chat_id))
                return;
            //this method to make a group and add a connection to it
           
            if (_context.ChatParticipants.Any(p => (p.UserId == sender_id) &&(chat_id == p.ChatId)))
			{
                var participant = _context.ChatParticipants
                                          .Include(p => p.User)
                                          .FirstOrDefault(p => p.UserId == sender_id);
                var sender_name = participant.User.FirstName+" "+participant.User.LastName;
                //save message to database
                var messageobj = new SignalR_Project.Models.Message
                {
                    SenderName = sender_name,
                    SenderId = sender_id,
                    Content = message,
                    ChatId = chat_id,
                    SentAt = DateTime.UtcNow,
                    StatusId = (int)MessageStatusEnum.Unread
                };
                _context.Messages.Add(messageobj);
                _context.SaveChanges();
				await Clients.Group(chat_id).SendAsync("ReceiveMessage", sender_id, message, chat_id, sender_name);
            }
			//this method to remove a connection from a group
			//Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
			//this method to send a message to all connection
			//Clients.All.SendAsync("ReceiveMessage", senderId, message);
		}
        //leave a group
        public async Task LeaveGroup(string chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
        }
        //this method is called when a new connection is established
        public override Task OnConnectedAsync()
		{
			var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
			var userConnection = new UserConnection
            {
                FirstName = user.FirstName,
                Email = user.Email,
                ConnectionId = Context.ConnectionId,
                UserId = userId,
                ConnectedAt = DateTime.UtcNow
            };
			_context.UserConnections.Add(userConnection);
			_context.SaveChanges();
            return base.OnConnectedAsync();
		}
		//this method is called when a connection is disconnected
		public override Task OnDisconnectedAsync(Exception exception)
		{
			var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
			var userConnection = _context.UserConnections.FirstOrDefault(uc => uc.ConnectionId == Context.ConnectionId && uc.UserId == userId);
            if (userConnection != null)
            {
                _context.UserConnections.Remove(userConnection);
                _context.SaveChanges();
            }
			return base.OnDisconnectedAsync(exception);
		}
	}
}
