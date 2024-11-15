using System;
using System.Collections.Generic;

namespace LibraryAzureFunctions2.Models
{
    public partial class BookCopy
    {
        public BookCopy()
        {
            BorrowRecords = new HashSet<BorrowRecord>();
        }

        public string Isbn { get; set; }
        public int BookId { get; set; }
        public bool IsBorrowed { get; set; }
        public string Edition { get; set; }
        public bool IsActive { get; set; }

        public virtual Book Book { get; set; }
        public virtual ICollection<BorrowRecord> BorrowRecords { get; set; }
    }
}
