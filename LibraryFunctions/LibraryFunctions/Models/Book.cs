using System;
using System.Collections.Generic;

namespace LibraryFunctions.Models;

public partial class Book
{
    public int BookId { get; set; }

    public int CategoryId { get; set; }

    public string Title { get; set; }

    public string Publisher { get; set; }

    public int PublishYear { get; set; }

    public string Description { get; set; }

    public virtual ICollection<BookCopy> BookCopies { get; set; } = new List<BookCopy>();

    public virtual Category Category { get; set; }

    public virtual ICollection<Author> Authors { get; set; } = new List<Author>();
}
