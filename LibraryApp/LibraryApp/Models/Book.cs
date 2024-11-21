using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models;

public partial class Book
{
    [Key]
    [Display(Name = "Book ID")]
    public int BookId { get; set; }
    
    [Display(Name = "Category ID")]
    public int CategoryId { get; set; }

    public string Title { get; set; } = null!;

    public string Publisher { get; set; } = null!;
    [Display(Name = "Publish Year")]
    public int PublishYear { get; set; }

    public string Description { get; set; } = null!;
    [Display(Name = "Book Copies")]
    public virtual ICollection<BookCopy> BookCopies { get; set; } = new List<BookCopy>();

    public virtual Category? Category { get; set; }

}
