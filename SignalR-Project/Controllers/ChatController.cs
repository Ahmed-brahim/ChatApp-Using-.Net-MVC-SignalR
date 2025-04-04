using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignalR_Project.Models;
using SignalR_Project.Models.Data;
using SignalR_Project.ViewModels;
using System.Security.Claims;

namespace SignalR_Project.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ChatController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var chatsViewModel = new List<ChatViewModel>();
            // Get the user ID from the claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Check if the user is authenticated
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            // Get the user from the database
            var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                var Userchats = _context.Chats.Where(c => c.Participants.Any(p => p.UserId == userId)).ToList();
                foreach (var chat in Userchats)
                {
                    var lastMessage = chat.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault();
                    var UnreadCount = chat.Messages.Count(m => (m.ChatId == chat.Id) && (m.SenderId != user.Id) && (m.StatusId != (int)MessageStatus.Seen));//status id = 3 means seen
                    chatsViewModel.Add(new ChatViewModel
                    {
                        Chat_Id = chat.Id,
                        Chat_Name = chat.Name,
                        Chat_Image = chat.IsGroupChat ? "/Groupicon.png" : "/DefaultProfilePhoto.webp", // Placeholder for chat image
                        LastMessage = lastMessage?.Content?? string.Empty,
                        LastMessageTime = lastMessage?.SentAt,
                        UnreadCount = UnreadCount
                    });
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            return View(chatsViewModel);
        }
        [HttpGet]
        public IActionResult NewChat()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> NewChat(NewChatViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Return the view with validation errors
                return View(model);
            }
            else{
                // Get the user ID from the claims
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                // get the user from the database
                var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
                //get the recipient user
                var recipientUser = _userManager.Users.FirstOrDefault(u => u.Email == model.UserEmail);

                //create a new chat
                var chat = new Chat();
                chat.Name = recipientUser.FirstName + (recipientUser.LastName?? " ");
                chat.IsGroupChat = false;
                //create new participants to store in database and add them to chat
                var participant1 = new ChatParticipant
                {
                    UserId = user.Id,
                    ChatId = chat.Id,
                    IsAdmin = true
                };
                var participant2 = new ChatParticipant
                {
                    UserId = recipientUser.Id,
                    ChatId = chat.Id,
                    IsAdmin = false
                };
                //add the participants to the chat
                chat.Participants.Add(participant1);
                chat.Participants.Add(participant2);
                //add the chat to the database
                _context.Chats.Add(chat);
                //save the changes to the database
                await _context.SaveChangesAsync();
                // Redirect to the chat page
                return RedirectToAction("Index", "Chat");

            }
			
        }
		[HttpGet]
		public IActionResult UserEmailCheck(string UserEmail)
		{
			var User = _userManager.Users.FirstOrDefault(u => (u.Email == UserEmail) && (u.IsDeleted != true));
			if (User == null)
			{
				return Json(false);
			}
			else
			{
				bool IsChatExist = _context.Chats.Any(c => c.Participants.Any(p => p.UserId == User.Id));
				if (IsChatExist)
				{
					return Json(false);
				}
				else
				{
					return Json(true);
				}
			}
		}
	}
    public enum MessageStatus
    {
        Unread = 1,
        Delivered = 2,
        Seen = 3
    }
}
