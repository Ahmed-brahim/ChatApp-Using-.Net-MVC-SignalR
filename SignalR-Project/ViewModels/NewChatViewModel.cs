using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SignalR_Project.ViewModels
{
	public class NewChatViewModel
	{
		
		[Required(ErrorMessage = "Chat name is required")]
		[DataType(DataType.EmailAddress, ErrorMessage ="invalid Email")]

		[Remote("UserEmailCheck", "Chat", ErrorMessage = "User not found or already in chat")]
		public string UserEmail { get; set; }
	}
}
