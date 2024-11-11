using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models;

public partial class Category
{
    [Key]
    [Display(Name = "Category ID")]
    public int CategoryId { get; set; }
    [Display(Name = "Category Name")]
    public string CategoryName { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
