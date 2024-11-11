using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models;

public partial class Role
{
    [Display(Name = "Role ID")]
    public int RoleId { get; set; }
    [Display(Name = "Role Name")]
    public string RoleName { get; set; } = null!;

    public virtual ICollection<Person> People { get; set; } = new List<Person>();
}
