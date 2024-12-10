using System;
using System.Collections.Generic;

namespace BookStore;

public partial class Store
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? StreetAdress { get; set; }

    public string? PostalCode { get; set; }

    public string? PostalAdress { get; set; }

    public virtual ICollection<StockBalance> StockBalances { get; set; } = new List<StockBalance>();
}
