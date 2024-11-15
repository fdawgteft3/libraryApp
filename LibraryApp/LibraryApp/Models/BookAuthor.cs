using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models;

public partial class BookAuthor
{
    [Display(Name = "Book ID")]
    [Key]
    public int BookId { get; set; }
    [Display(Name = "Author ID")]
    [Key]
    public int AuthorId { get; set; }

    public virtual Author Author { get; set; } = null!;

    public virtual Book Book { get; set; } = null!;
}
