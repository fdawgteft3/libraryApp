using System;
using System.Collections.Generic;

namespace LibraryAzureFunctions.Models;

public partial class Author
{
    public int AuthorId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;
}
