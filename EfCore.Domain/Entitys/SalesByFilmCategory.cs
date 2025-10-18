using System;
using System.Collections.Generic;

namespace EfCore.Domain.Entitys;

public partial class SalesByFilmCategory
{
    public string? Category { get; set; }

    public decimal? TotalSales { get; set; }
}
