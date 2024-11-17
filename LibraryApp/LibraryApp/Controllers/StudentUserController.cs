using LibraryApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Controllers
{
    public class StudentUserController : Controller
    {
        //UserManager and RoleManager to manage user and roles
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public StudentUserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> StudentUsers()
        {
            // Fetch all users
            var users = await _userManager.Users.ToListAsync();
            // Prepare the model list
            var model = new List<AssignRoleViewModel>();
            // Loop through users
            foreach (var user in users)
            {
                // Get roles for the current user
                var userRoles = await _userManager.GetRolesAsync(user);
                // Check if the user has the "Student" role
                if (userRoles.Contains("Student"))
                {
                    // Add user to the model if the "Student" role is present
                    model.Add(new AssignRoleViewModel
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        //Maybe need add here
                        Roles = _roleManager.Roles.ToList(),
                        SelectedRoleIds = userRoles.ToList()
                    });
                }
            }
            // Return the view with the filtered model
            return View(model);
        }
    }
}
