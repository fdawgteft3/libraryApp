using System;
using System.Collections.Generic;

namespace LibraryAzureFunctions.Models;

public partial class Person
{
    public int MemberId { get; set; }

    public string PfirstName { get; set; } = null!;

    public string PlastName { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public int RoleId { get; set; }

    public virtual ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();

    public virtual Role Role { get; set; } = null!;
}
