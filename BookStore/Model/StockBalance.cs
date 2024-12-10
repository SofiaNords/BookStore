using System;
using System.Collections.Generic;

namespace BookStore;

public partial class StockBalance
{
    public int StoreId { get; set; }

    public long Isbn13 { get; set; }

    public int? Amount { get; set; }

    public virtual Book Isbn13Navigation { get; set; } = null!;

    public virtual Store Store { get; set; } = null!;
}
