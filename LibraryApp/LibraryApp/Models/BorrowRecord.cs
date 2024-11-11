using System;
using System.Collections.Generic;

namespace LibraryApp.Models;

public partial class BorrowRecord
{
    public int RecordNumber { get; set; }

    public int MemberId { get; set; }

    public string Isbn { get; set; } = null!;

    public DateTime DateBorrowed { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime? DateReturned { get; set; }

    public decimal LateFee { get; set; }

    public decimal? OustandingFee { get; set; }

    public decimal? AmountPaid { get; set; }

    public bool IsActive { get; set; }

    public virtual BookCopy IsbnNavigation { get; set; } = null!;

    public virtual Person Member { get; set; } = null!;
}
