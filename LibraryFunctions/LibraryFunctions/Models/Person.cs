using System;
using System.Collections.Generic;

namespace LibraryFunctions.Models;

public partial class Person
{
    public int MemberId { get; set; }

    public string PfirstName { get; set; }

    public string PlastName { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public string Email { get; set; }

    public string Phone { get; set; }

    public int RoleId { get; set; }

    public virtual ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();

    public virtual Role Role { get; set; }
}
