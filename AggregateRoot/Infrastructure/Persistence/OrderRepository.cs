using System;
using System.Threading.Tasks;
using AggregateRoot.Domain.Orders;
using Microsoft.EntityFrameworkCore;

namespace AggregateRoot.Infrastructure.Persistence
{
    public sealed class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order?> GetByIdAsync(Guid orderId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<Order?> GetByOrderNumberAsync(string orderNumber)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
        }

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(Guid orderId)
        {
            return await _context.Orders.AnyAsync(o => o.Id == orderId);
        }

        public async Task<string> GenerateOrderNumberAsync()
        {
            var count = await _context.Orders.CountAsync();
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{count + 1:D6}";
        }
    }
}