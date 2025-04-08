using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalR_Project.Hubs;
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
        private readonly IHubContext<ChatHub> _hubContext;
        public ChatController(AppDbContext context, UserManager<ApplicationUser> userManager, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
        }
        public async Task<IActionResult> Index()
        {
            var chatsViewModel = new List<ChatDashBoardViewModel>();
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
                var Userchats = _context.Chats.Include(c => c.Participants).ThenInclude(p => p.User).Include(c => c.Messages).Where(c => c.Participants.Any(p => p.UserId == userId)).ToList();
                foreach (var chat in Userchats)
                {
                    var lastMessage = chat.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault();
                    var UnreadCount = chat.Messages.Count(m => (m.ChatId == chat.Id) && (m.SenderId != user.Id) && (m.StatusId != (int)MessageStatusEnum.Seen));//status id = 3 means seen
                    var chatparticipants = chat.Participants.ToList();
                    string chatName = string.Empty;
                    if (chat.IsGroupChat)
                    {
                        chatName = chat.Name;
                    }
                    else {
                        if (!chatparticipants.Any(p => p.UserId != userId))
                        {
                            chatName = chat.Name;
                        }
                        else
                        {
                            chatName = chat.IsGroupChat ? chat.Name : chat.Participants.FirstOrDefault(p => p.UserId != userId)?.User?.FirstName + " " + chat.Participants.FirstOrDefault(p => p.UserId != userId)?.User?.LastName;

                        }
                        //chatName = chat.Participants.FirstOrDefault(p => p.UserId != user.Id)?.User?.FirstName + " " + chat.Participants.FirstOrDefault(p => p.UserId != user.Id)?.User?.LastName;
                     }
                    chatsViewModel.Add(new ChatDashBoardViewModel
                    {
                        Chat_Id = chat.Id,
                        Chat_Name = chatName,
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
                if (userId == recipientUser.Id)
                {
                    chat.Name = user.FirstName +" "+ "(ME)";
                }
                else
                {
                    chat.Name = recipientUser.FirstName +" "+ (recipientUser.LastName ?? " ");
                }
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
        public IActionResult CreateGroup()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup(NewGroupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("NewGroup", model);
            }
            // Get the user ID from the claims
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // get the user from the database
            var user = _userManager.Users.FirstOrDefault(u => u.Id == UserId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //create a new chat
            var chat = new Chat
            {
                Name = model.GroupName,
                IsGroupChat = true,
                CreatedAt = DateTime.UtcNow,
                Participants = new List<ChatParticipant>()
                {
                    new ChatParticipant(){
                        UserId = user.Id,
                        IsAdmin = true
                    }
                }
            };
            //add the chat to the database
            _context.Chats.Add(chat);
            //save the changes to the database
            await _context.SaveChangesAsync();
            // Redirect to the chat page
            return RedirectToAction("Index", "Chat");
        }
        [HttpGet]
        public IActionResult OpenChat(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(_context.ChatParticipants.Any(p => (p.UserId==userId) && (p.ChatId == id)))
            {
                var model = new ChatViewModel();
                var chat = _context.Chats.Include(c => c.Participants).ThenInclude(p => p.User)
                                            .Include(c=> c.Messages).FirstOrDefault(c => c.Id == id);
                var chatparticipants = chat.Participants.ToList();
                string chatName = string.Empty;
                if (!chat.IsGroupChat)
                {
                    if (!chatparticipants.Any(p => p.UserId != userId))
                    {
                        chatName = chat.Name;
                    }
                    else
                    {
                        chatName = chat.IsGroupChat ? chat.Name : chat.Participants.FirstOrDefault(p => p.UserId != userId)?.User?.FirstName + " " + chat.Participants.FirstOrDefault(p => p.UserId != userId)?.User?.LastName;

                    }
                }
                else
                {
                    chatName = chat.IsGroupChat ? chat.Name : chat.Participants.FirstOrDefault(p => p.UserId != userId)?.User?.FirstName + " " + chat.Participants.FirstOrDefault(p => p.UserId != userId)?.User?.LastName;

                }
                model.Chat_Image = chat.IsGroupChat ? "/Groupicon.png" : "/DefaultProfilePhoto.webp"; // Placeholder for chat image
                model.Chat_Id = chat.Id;
                model.IsGroupChat = chat.IsGroupChat;
                model.Chat_Name = chatName;
                model.Messages = chat.Messages.Select(m => new MessageViewModel2
                {
                    Id = m.Id,
                    SenderName = m.SenderId == User.FindFirstValue(ClaimTypes.NameIdentifier) ? "You" : chat.Participants.FirstOrDefault(p => p.UserId == m.SenderId)?.User?.FirstName + " " + chat.Participants.FirstOrDefault(p => p.UserId == m.SenderId)?.User?.LastName,
                    SenderId = m.SenderId,
                    Text = m.Content,
                    TimeSent = m.SentAt
                }).OrderBy(m => m.TimeSent).ToList();
                model.Participants = chat.Participants.Select(p => new ParticipantViewModel
                {
                    ParticipantName = p.User.FirstName + " " + p.User.LastName,
                    ParticipantId = p.User.Id,
                    ParticipantImage = "/DefaultProfilePhoto.webp" // Placeholder for participant image
                }).ToList();
                return View("OpenChat", model);
            }
            else
            {
                return RedirectToAction("Index", "Chat");
            }
        }
        /******************************************************************/
        [HttpGet]
        public async Task<IActionResult> SearchUsers(string term, string chatId)
        {
            if (string.IsNullOrEmpty(term))
            {
                return BadRequest("Search term is required");
            }

            // Get existing participant IDs for the chat if chatId is provided
            List<string> existingParticipantIds = new List<string>();
            if (!string.IsNullOrEmpty(chatId))
            {
                existingParticipantIds = await _context.ChatParticipants
                    .Where(cp => cp.ChatId == chatId)
                    .Select(cp => cp.UserId)
                    .ToListAsync();
            }

            // Search for users by name or email, excluding existing participants
            var users = await _userManager.Users
                .Where(u => (u.UserName.Contains(term) || u.Email.Contains(term)) &&
                           !existingParticipantIds.Contains(u.Id))
                .Select(u => new {
                    id = u.Id,
                    name = u.UserName,
                    email = u.Email,
                    profileImage = u.image // Using the property name from your code
                })
                .Take(10)
                .ToListAsync();

            return Json(users);
        }

        // Invite participants to chat
        [HttpPost]
        public async Task<IActionResult> InviteParticipant(string chatId, string userIds)
        {
            if (string.IsNullOrEmpty(userIds))
            {
                return BadRequest("No users selected");
            }

            var selectedUserIds = userIds.Split(',');
            var chat = await _context.Chats.FirstOrDefaultAsync(c => c.Id == chatId);

            if (chat == null)
            {
                return NotFound("Chat not found");
            }

            // Get existing participants to avoid duplicates
            var existingParticipantIds = await _context.ChatParticipants
                .Where(cp => cp.ChatId == chatId)
                .Select(cp => cp.UserId)
                .ToListAsync();

            // Track if any new participants were added
            bool participantsAdded = false;

            // Add users to chat
            foreach (var userId in selectedUserIds)
            {
                // Skip if user is already a participant
                if (existingParticipantIds.Contains(userId))
                {
                    continue;
                }

                var chatParticipant = new ChatParticipant
                {
                    UserId = userId,
                    ChatId = chatId,
                    IsAdmin = false // Set to true if the user should be an admin
                };

                _context.ChatParticipants.Add(chatParticipant);
                participantsAdded = true;

                // Get user details for notification
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    // Notify other participants via SignalR
                    await _hubContext.Clients.Group(chatId).SendAsync(
                        "ParticipantJoined",
                        userId,
                        user.UserName,
                        user.image ?? "/placeholder.svg?height=30&width=30"
                    );
                }
            }

            // Save changes once at the end if any participants were added
            if (participantsAdded)
            {
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("OpenChat", new { id = chatId });
        }

        // Add this method to check if a user exists and is already a participant
        [HttpGet]
        public async Task<IActionResult> CheckIfUserExists(string term, string chatId)
        {
            if (string.IsNullOrEmpty(term) || string.IsNullOrEmpty(chatId))
            {
                return BadRequest("Term and chat ID are required");
            }

            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.UserName.Contains(term) || u.Email.Contains(term));

            if (user == null)
            {
                return Json(new { exists = false });
            }

            var isParticipant = await _context.ChatParticipants
                .AnyAsync(cp => cp.ChatId == chatId && cp.UserId == user.Id);

            return Json(new
            {
                exists = true,
                isParticipant = isParticipant,
                name = user.UserName
            });
        }

        [HttpPost]
        public async Task<IActionResult> LeaveGroup(string chatId, string userId)
        {
            if (string.IsNullOrEmpty(chatId) || string.IsNullOrEmpty(userId))
            {
                return BadRequest("Chat ID and User ID are required");
            }

            // Get the current user
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Security check - ensure the user is only removing themselves
            if (currentUserId != userId)
            {
                return Forbid("You can only remove yourself from the group");
            }

            // Get the participant to remove
            var participant = await _context.ChatParticipants
                .FirstOrDefaultAsync(cp => cp.ChatId == chatId && cp.UserId == userId);

            if (participant == null)
            {
                return NotFound("Participant not found in this chat");
            }

            // Get user details for notification before removing
            var user = await _userManager.FindByIdAsync(userId);
            var userName = user?.UserName ?? "A participant";

            // Remove the participant
            _context.ChatParticipants.Remove(participant);
            await _context.SaveChangesAsync();

            // Notify other participants via SignalR
            await _hubContext.Clients.Group(chatId).SendAsync(
                "ParticipantLeft",
                userId,
                userName
            );

            // Redirect to chats list or home page
            return RedirectToAction("Index", "Home");
        }
        /********************************************************************/

        [HttpGet]
		public IActionResult UserEmailCheck(string UserEmail)
		{
			var ChatUser = _userManager.Users.FirstOrDefault(u => (u.Email == UserEmail) && (u.IsDeleted != true));
            if (ChatUser == null)
			{
				return Json(false);
			}
			else
			{
               var CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                bool IsChatExist = _context.Chats.Where(c => c.IsGroupChat == false).Any(c => c.Participants.Any(p => p.UserId == ChatUser.Id) && (c.Participants.Any(p => p.UserId == CurrentUserId)));
				//bool IsChatExist = _context.Chats.Include(c => c.Participants).Any(c => c.Participants.Any(p => p.UserId == User.Id));
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
        [HttpGet]
        public IActionResult UserEmailCheckForGroup(string userEmail)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Email == userEmail && !u.IsDeleted);
            return Json(user != null); // If user found, return true; else false
        }
    }
   
}
