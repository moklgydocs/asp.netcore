using System;
using System.Collections.Generic;

namespace EfCore.Application.Contracts.Dtos;

public partial class CustomerDto
{
    public int Index { get; set; }
    public int CustomerId { get; set; }

    public int StoreId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Email { get; set; }

    public int AddressId { get; set; }

    public bool Activebool { get; set; }

    public DateOnly CreateDate { get; set; }

    public DateTime? LastUpdate { get; set; }

    public int? Active { get; set; }

}
