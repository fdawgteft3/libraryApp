using System;
using System.Collections.Generic;

namespace LibraryAzureFunctions.Models;

public partial class Book
{
    public int BookId { get; set; }

    public int CategoryId { get; set; }

    public string Title { get; set; } = null!;

    public string Publisher { get; set; } = null!;

    public int PublishYear { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<BookCopy> BookCopies { get; set; } = new List<BookCopy>();

    public virtual Category Category { get; set; } = null!;
}
