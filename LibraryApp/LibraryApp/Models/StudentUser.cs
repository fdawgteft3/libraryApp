namespace LibraryApp.Models
{
    public class StudentUser
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public virtual ICollection<Borrow_Record> Borrow_Records { get; set; }
        //Extra Parameters for the Search by Date Range Azure Function
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
    }
}
