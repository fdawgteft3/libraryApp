using System;
using System.Collections.Generic;

namespace LibraryFunctions.Models;

public partial class BookCopy
{
    public string Isbn { get; set; }

    public int BookId { get; set; }

    public bool IsBorrowed { get; set; }

    public string Edition { get; set; }

    public bool IsActive { get; set; }

    public virtual Book Book { get; set; }

    public virtual ICollection<Borrow_Record> Borrow_Records { get; set; } = new List<Borrow_Record>();
}
