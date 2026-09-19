using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        public AccountsController(SignInManager<IdentityUser> signInManager)
        {
            this._signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM login)
        {
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                try
                {
                    var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, login.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");

                        // return LocalRedirect(returnUrl);
                    }

                    //if (result.RequiresTwoFactor)
                    //{
                    //    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = login.RememberMe });
                    //}
                    //if (result.IsLockedOut)
                    //{

                    //    return RedirectToPage("./Lockout");
                    //}
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");

                    }
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"An error occurred while processing your request.{ex.InnerException.Message??ex.Message}");
                    // Log the exception (ex) as needed
                }
            }
            return View(login);
        }
        [HttpPost]
        public async Task<IActionResult> Logout(string? returnUrl = null)
        {
            await _signInManager.SignOutAsync();
          
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                
                return RedirectToAction("Index", "Home");
            }

            
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
