using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models;

public partial class BookCopy
{
    public string ISBN { get; set; } = null!;
    [Display(Name = "Book ID")]
    public int BookId { get; set; }

    public bool IsBorrowed { get; set; }

    public string Edition { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual Book Book { get; set; } = null!;


    public virtual ICollection<Borrow_Record> Borrow_Records { get; set; } = new List<Borrow_Record>();
}
