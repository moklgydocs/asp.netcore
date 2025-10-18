using System;
using System.Collections.Generic;

namespace EfCore.Domain.Entitys;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int CustomerId { get; set; }

    public int StaffId { get; set; }

    public int RentalId { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Rental Rental { get; set; } = null!;

    public virtual Staff Staff { get; set; } = null!;
}
