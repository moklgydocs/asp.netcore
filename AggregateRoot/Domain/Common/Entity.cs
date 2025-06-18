using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AggregateRoot.Domain.Common
{
    public abstract class Entity<TId> : IEquatable<Entity<TId>>
        where TId : notnull
    {
        public TId Id { get; protected set; }

        private readonly List<IDomainEvent> _domainEvents = new();

        [NotMapped]
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected Entity(TId id)
        {
            Id = id;
        }

        protected Entity() { } // For EF Core

        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        public bool Equals(Entity<TId>? other)
        {
            return other is not null && Id.Equals(other.Id);
        }

        public override bool Equals(object? obj)
        {
            return obj is Entity<TId> entity && Equals(entity);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
        {
            return !Equals(left, right);
        }
    }
}