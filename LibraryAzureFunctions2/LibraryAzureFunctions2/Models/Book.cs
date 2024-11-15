using System;
using System.Collections.Generic;

namespace LibraryAzureFunctions2.Models
{
    public partial class Book
    {
        public Book()
        {
            BookCopies = new HashSet<BookCopy>();
            Authors = new HashSet<Author>();
        }

        public int BookId { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string Publisher { get; set; }
        public int PublishYear { get; set; }
        public string Description { get; set; }

        public virtual Category Category { get; set; }
        public virtual ICollection<BookCopy> BookCopies { get; set; }

        public virtual ICollection<Author> Authors { get; set; }
    }
}
