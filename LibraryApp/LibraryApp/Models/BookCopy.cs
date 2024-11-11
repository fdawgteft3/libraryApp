using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models;

public partial class BookCopy
{
    public string Isbn { get; set; } = null!;
    [Display(Name = "Book ID")]
    public int BookId { get; set; }

    public bool IsBorrowed { get; set; }

    public string Edition { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();
}
