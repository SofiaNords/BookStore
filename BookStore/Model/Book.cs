using System;
using System.Collections.Generic;

namespace BookStore;

public partial class Book
{
    public long Isbn13 { get; set; }

    public string Title { get; set; } = null!;

    public string Language { get; set; } = null!;

    public decimal Price { get; set; }

    public int YearOfPublication { get; set; }

    public int AuthorId { get; set; }

    public int GenreId { get; set; }

    public int PublisherId { get; set; }

    public virtual Author Author { get; set; } = null!;

    public virtual Genre Genre { get; set; } = null!;

    public virtual Publisher Publisher { get; set; } = null!;

    public virtual ICollection<StockBalance> StockBalances { get; set; } = new List<StockBalance>();
}
