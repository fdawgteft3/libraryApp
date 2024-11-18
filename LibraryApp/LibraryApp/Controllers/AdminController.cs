using LibraryApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LibraryApp.Controllers
{
    public class AdminController : Controller
    {
        //UserManager and RoleManager to manage user and roles
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            //Retrieve all roles from DB
            var roles = _roleManager.Roles.ToList();
            return View(roles); //Pass the roles to the view
        }

        //Create a new role
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        //process the form submission to create a new role
        [HttpPost]
        public async Task<IActionResult> CreateRole(IdentityRole role)
        {
            //Check if the role alraady exists
            if (await _roleManager.RoleExistsAsync(role.Name))
            {
                ModelState.AddModelError("", "Role already exists");
                return View(role);
            }
            //create the role in the database
            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                role.ConcurrencyStamp = Guid.NewGuid().ToString();
                await _roleManager.UpdateAsync(role);
                return RedirectToAction("Index");
            }
            return View(role);
        }

        //Edit Roles
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            //Retrieve the role by id
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();

            }
            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(IdentityRole model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                return NotFound();

            }
            role.Name = model.Name; //update role name
            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            return View(model); //Return view with error
        }

        // Display confirmation page for deleting a role
        // Delete Role 
        [HttpGet]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id); // Retrieve the role by ID
            if (role == null) return NotFound(); // Return 404 if role not found

            return View(role); // Pass the role to the view
        }

        // Confirm deletion of a role
        [HttpPost, ActionName("DeleteRole")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);  // Retrieve the role by ID
            if (role == null) return NotFound(); // Return 404 if role not found

            var result = await _roleManager.DeleteAsync(role); // Delete the role
            if (result.Succeeded) return RedirectToAction("Index"); // Redirect if deletion successful

            // Optionally, you can return the same view with errors if needed
            ModelState.AddModelError(string.Empty, "Unable to delete the role.");
            return RedirectToAction("Index"); // Redirect if deletion fails
        }


        //Crud For User
        //Display a list of all users with thier assigned roles
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var model = new List<AssignRoleViewModel>();
            //loop through users to populatie the model with roles
            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var roles = _roleManager.Roles.ToList();
                model.Add(new AssignRoleViewModel //New class model
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Roles = roles,
                    SelectedRoleIds = userRoles.ToList()
                });

            }
            return View(model);
        }

        //---------------------------------------------------------

        // Display the "Create User" form
        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        // Process the form submission to create a new user
        [HttpPost]
        public async Task<IActionResult> CreateUser(string userName, string email, string password)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = userName,
                    Email = email
                };

                // // Create the user in the database with password
                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    // Set the EmailConfirmed property to true
                    user.EmailConfirmed = true;

                    // Update the user in the database to save the changes
                    var updateResult = await _userManager.UpdateAsync(user);

                    if (updateResult.Succeeded)
                    {
                        return RedirectToAction("Users"); // Redirect to user list if successful
                    }
                    else
                    {
                        // Handle any errors during update
                        foreach (var error in updateResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }

                // Add errors to ModelState if creation failed
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(); // Return the form with errors if needed
        }

        //---------------------------------------------------------

        // Display the "Assign Role" form for a specific user
        // Assign roles to a user
        [HttpGet]
        public async Task<IActionResult> AssignRole(string id)
        {
            var user = await _userManager.FindByIdAsync(id); // Retrieve the user by ID
            if (user == null) return NotFound(); // Return 404 if user not found

            var roles = await _roleManager.Roles.ToListAsync(); // Retrieve all roles
            var userRoles = await _userManager.GetRolesAsync(user); // Get user's current roles

            var model = new AssignRoleViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roles = roles,
                SelectedRoleIds = userRoles.ToList()  // Pre-select current roles
            };

            return View(model); // Pass the model to the view
        }

        // Process the role assignment form submission
        [HttpPost]
        public async Task<IActionResult> AssignRole(AssignRoleViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId); // Retrieve the user by ID
            if (user == null) return NotFound(); // Return 404 if user not found

            // Remove current roles // Remove all current roles from the user
            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                foreach (var error in removeResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            // Add the new selected roles  // Add the new selected roles to the user
            if (model.SelectedRoleIds == null)
            {
                ModelState.AddModelError("", "No role selected");
                return View(model);
            }
            //Redirect to AddStudent View if student selected
            if(model.SelectedRoleIds.Contains("Student")||model.SelectedRoleIds.Contains("student"))
            {
                return RedirectToAction("AddStudent","Student");
            }
            var addResult = await _userManager.AddToRolesAsync(user, model.SelectedRoleIds);

            if (addResult.Succeeded)
            {
                return RedirectToAction("Users"); // Redirect to user list if successful
            }

            // Add errors to ModelState if adding roles fails
            foreach (var error in addResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);  // Return view with errors if needed
        }
    }
}

