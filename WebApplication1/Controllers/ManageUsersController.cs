using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class ManageUsersController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IRoleStore<IdentityRole> _roleStore;
        public ManageUsersController(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserStore<IdentityUser> userStore,
            IRoleStore<IdentityRole> roleStore,
            ApplicationDBContext context)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._userStore = userStore;
            this._roleStore = roleStore;
            this._context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View("Register");
        }
        [HttpPost]
        public async Task<IActionResult> Create(RegisterVM register)
        {
            var user = new IdentityUser
            {
                UserName = register.Email,
                Email = register.Email
            };
            var result = await _userManager.CreateAsync(user, register.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else if (result.Errors.Count() > 0)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            else
            {
                ModelState.AddModelError("", "user creation failed for unknown reasons.");
            }
            return View("Register", register);
        }

        public async Task<IActionResult> AssignRole( )
        {
            ViewBag.userEmail =new SelectList(  _userManager.Users.ToList(), "Email", "Email" );
            ViewBag.roles =new SelectList(  _roleManager.Roles.ToList(), "Name", "Name" );
            return View();
        }
            [HttpPost]
        public async Task<IActionResult> AssignRole(string useremail, string role)
        {
            var user = await _userManager.FindByEmailAsync(useremail);
            var result = await _userManager.AddToRoleAsync(user, role);
            if (result.Succeeded)
            {
                return RedirectToAction("UserRole", "ManageUsers");
            }
            else if (result.Errors.Count() > 0)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View();
        }
        public async Task<IActionResult> UserRole(string useremail, string role)
        {

            var usersWithRoles = await (from user in _context.Users
                                        select new UserWithRolesViewModel
                                        {
                                            UserId = user.Id,
                                            Username = user.UserName,
                                            Email = user.Email,
                                            Roles = (from userRole in _context.UserRoles
                                                     join role in _context.Roles on userRole.RoleId equals role.Id
                                                     where userRole.UserId == user.Id
                                                     select role.Name).ToList()
                                        }).ToListAsync();

            return View(usersWithRoles);
            
        }
    }
}
