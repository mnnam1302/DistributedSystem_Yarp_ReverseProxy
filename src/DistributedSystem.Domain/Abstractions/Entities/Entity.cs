namespace DistributedSystem.Domain.Abstractions.Entities
{
    public abstract class Entity<T> : IEntity<T>
    {
        public T Id { get; set; }
        public bool IsDeleted { get; protected set; }
    }

    //public abstract class DomainEntity<T> : IEntity<T>
    //{
    //    public T Id { get; protected set; }

    //    // Ví dụ, có thể Audit ở đây luôn Created and Modified
    //}
}