namespace DistributedSystem.Domain.Abstractions.Entities
{
    public interface IEntity<T>
    {
        T Id { get; set; }
    }
}