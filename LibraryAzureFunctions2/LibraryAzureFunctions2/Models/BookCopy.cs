using System;
using System.Collections.Generic;

namespace LibraryAzureFunctions2.Models
{
    public partial class BookCopy
    {
        public BookCopy()
        {
            Borrow_Records = new HashSet<Borrow_Record>();
        }

        public string Isbn { get; set; }
        public int BookId { get; set; }
        public bool IsBorrowed { get; set; }
        public string Edition { get; set; }
        public bool IsActive { get; set; }

        public virtual Book Book { get; set; }
        public virtual ICollection<Borrow_Record> Borrow_Records { get; set; }
    }
}
