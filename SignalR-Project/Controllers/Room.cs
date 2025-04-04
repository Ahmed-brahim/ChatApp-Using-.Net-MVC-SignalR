using Microsoft.AspNetCore.Mvc;

namespace SignalR_Project.Controllers
{
	public class Room : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
