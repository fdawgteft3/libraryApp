using System;
using System.Collections.Generic;

namespace LibraryAzureFunctions.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<Person> People { get; set; } = new List<Person>();
}
