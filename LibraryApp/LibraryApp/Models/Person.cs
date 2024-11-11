using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models;

public partial class Person
{
    [Key]
    [Display(Name = "Member ID")]
    public int MemberId { get; set; }
    [Display(Name = "Member First Name")]
    public string PfirstName { get; set; } = null!;
    [Display(Name = "Member Last Name")]
    public string PlastName { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;
    [Display(Name = "Role ID")]
    public int RoleId { get; set; }
    [Display(Name = "Borrow Records")]
    public virtual ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();

    public virtual Role Role { get; set; } = null!;
}
