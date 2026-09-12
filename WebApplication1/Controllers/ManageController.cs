using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class ManageController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IRoleStore<IdentityRole> _roleStore;
        public ManageController(RoleManager<IdentityRole> roleManager, IRoleStore<IdentityRole> roleStore)
        {
            _roleManager = roleManager;
            _roleStore = roleStore;
        }
        public IActionResult Index()
        {
            var model = _roleManager.Roles.ToList();
            var allroles = new List<ROleVm>();
            

            foreach (var item in model  )
            {
                allroles.Add(new ROleVm { Name = item.Name, ID = item.Id });
            }
     
            return View(allroles);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ROleVm roleVm)
        {
            if (ModelState.IsValid)
            {
                try

                {
                    var result = await _roleManager.CreateAsync(new IdentityRole { Name = roleVm.Name });
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    if (result.Errors.Any())
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }

                }

                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    //   return View(roleVm);

                }
            }
           
            else
            {
                var errorMessages = ModelState.Values
         .SelectMany(v => v.Errors)
         .Select(e => e.ErrorMessage)
         .ToList();
                ModelState.AddModelError(string.Empty, string.Join(", ", errorMessages));
            }
            return View(roleVm);
        }
    }
}
