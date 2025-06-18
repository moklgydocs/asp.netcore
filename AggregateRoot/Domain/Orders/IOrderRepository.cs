using System;
using System.Threading.Tasks;

namespace AggregateRoot.Domain.Orders
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid orderId);
        Task<Order?> GetByOrderNumberAsync(string orderNumber);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task<bool> ExistsAsync(Guid orderId);
        Task<string> GenerateOrderNumberAsync();
    }
}