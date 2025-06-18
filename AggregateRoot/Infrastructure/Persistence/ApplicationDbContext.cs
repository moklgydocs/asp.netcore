using AggregateRoot.Domain.Orders;
using Microsoft.EntityFrameworkCore;

namespace AggregateRoot.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; } = null!; // Assuming Order is an entity in your domain

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}