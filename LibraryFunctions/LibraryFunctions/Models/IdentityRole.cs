using System;
using System.Collections.Generic;

namespace LibraryFunctions.Models;

public partial class IdentityRole
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string NormalizedName { get; set; }

    public string ConcurrencyStamp { get; set; }

    public string StudentUserUserId { get; set; }

    public virtual StudentUser StudentUserUser { get; set; }
}
