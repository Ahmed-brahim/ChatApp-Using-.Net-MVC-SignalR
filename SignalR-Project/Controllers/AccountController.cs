using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignalR_Project.Models;
using SignalR_Project.ViewModels;
using System.Security.Claims;

namespace SignalR_Project.Controllers
{
    public class AccountController : Controller
    {
        UserManager<ApplicationUser> _userManager;
        SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
               //find user by email
               ApplicationUser user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                { 
                    bool found = await _userManager.CheckPasswordAsync(user, model.Password);
                    if(found) //correct password
                    {
                        //add claims
                        var claims = new List<Claim> { 
                            new Claim("FirstName", user.FirstName) 
                        };
                        //sign in
                        await _signInManager.SignInWithClaimsAsync(user, isPersistent: model.RememberMe, claims);
                        return RedirectToAction("Index", "Home");
                    }
                    else  //wrong password
                    {
                        ModelState.AddModelError("", "Invalid Login Attempt");
                    }
                }
                else //user not found  
                {
                   ModelState.AddModelError("", "Invalid Login Attempt");
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser();
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.UserName = model.Email;
                user.Country = model.Country;
                user.image = "/DefaultProfilePhoto.webp";

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded) {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim("FirstName", user.FirstName)
                    };
                    await _signInManager.SignInWithClaimsAsync(user, isPersistent: false, claims);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }
        
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
