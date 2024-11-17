using System;
using System.Collections.Generic;

namespace LibraryFunctions.Models;

public partial class StudentUser
{
    public string UserId { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string SelectedRoleIds { get; set; }

    public virtual ICollection<IdentityRole> IdentityRoles { get; set; } = new List<IdentityRole>();
}
