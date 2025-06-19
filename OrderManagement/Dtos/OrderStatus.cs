namespace OrderManagement.Domain.Entities
{
    /// <summary>
    /// ¶©µ¥×´Ì¬Ã¶¾Ù
    /// </summary>
    public enum OrderStatus
    {
        Draft = 0,
        Confirmed = 1,
        Paid = 2,
        Shipped = 3,
        Completed = 4,
        Cancelled = 5
    }
}
