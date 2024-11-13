using LibraryApp.Models;

namespace LibraryApp.Models.ViewModel
{
    public class CategoryBookAuthors
    {
        public Category Category { get; set; }
        public List<BookAuthors> Books { get; set; }
    }
}
