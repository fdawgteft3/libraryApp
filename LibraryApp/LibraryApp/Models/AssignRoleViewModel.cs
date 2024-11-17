using Microsoft.AspNetCore.Identity;

namespace LibraryApp.Models
{
    public class AssignRoleViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        //A list of availabel roles


        public string FirstName { get; set; }
        public string LastName { get; set; }



        public List<IdentityRole> Roles { get; set; }
        //List of selected role IDs for the user
        public List<string> SelectedRoleIds { get; set; }
    }
}
