namespace LibraryApp.Models.ViewModel
{
    public class BorrowVM
    {
        public StudentUser user { get; set; }
        public BookCopy bc { get; set; }    
        public Book bk { get; set; }
        public Borrow_Record record { get; set; }
    }
}
