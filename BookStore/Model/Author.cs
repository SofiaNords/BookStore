using System;
using System.Collections.Generic;

namespace BookStore;

public partial class Author
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string FullName
    {
        get
        {
            return $"{FirstName} {LastName}";
        }
    }

    public DateOnly? DateOfBirth { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
