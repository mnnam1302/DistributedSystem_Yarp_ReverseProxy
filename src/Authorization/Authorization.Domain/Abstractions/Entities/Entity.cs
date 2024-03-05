namespace Authorization.Domain.Abstractions.Entities
{
    public abstract class Entity<TKey> : IEntity<TKey>
    {
        public TKey Id { get; protected set; }
    }
}