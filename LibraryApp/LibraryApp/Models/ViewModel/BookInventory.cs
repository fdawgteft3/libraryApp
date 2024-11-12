using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models.ViewModel
{
    public class BookInventory
    {
        [Key]
        public string ISBN { get; set; } = null!;
        public string Edition { get; set; } = null!;
        public bool IsBorrowed { get; set; }
        public string Title { get; set; } = null!;
        public string Publisher { get; set; } = null!;
        [Display(Name = "Publish Year")]
        public int PublishYear { get; set; }
        public string Description { get; set; } = null!;
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = null!;
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = null!;

    }
}
