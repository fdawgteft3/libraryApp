using System;
using System.Collections.Generic;

namespace LibraryFunctions.Models;

public partial class BookInventory
{
    public string Isbn { get; set; }

    public string Edition { get; set; }

    public bool IsBorrowed { get; set; }

    public string Title { get; set; }

    public string Publisher { get; set; }

    public int PublishYear { get; set; }

    public string Description { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }
}
