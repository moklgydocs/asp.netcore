using System;

namespace AggregateRoot.Domain.Common
{
    public abstract class AggregateRoot<TId> : Entity<TId>
        where TId : notnull
    {
        public DateTime CreatedAt { get; protected set; }
        public DateTime? UpdatedAt { get; protected set; }
        public string CreatedBy { get; protected set; }
        public string? UpdatedBy { get; protected set; }

        protected AggregateRoot(TId id) : base(id)
        {
            CreatedAt = DateTime.UtcNow;
        }

        protected AggregateRoot() : base() { } // For EF Core

        protected void UpdateTimestamp(string updatedBy)
        {
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = updatedBy;
        }
    }
}