using Microsoft.AspNetCore.Mvc;

namespace SignalR_Project.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return View("index");
        }
    }
}
