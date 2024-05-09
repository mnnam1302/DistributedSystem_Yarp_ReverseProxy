using DistributedSystem.Contract.Abstractions.Message;

namespace DistributedSystem.Contract.Services.V1.Product;

public static class DomainEvent
{
    // Kế thừa từ interface cha IDomainEvent
    // Tất cả class mà kế thừa từ INTERFACE IDomainEvent đều có thể add vào List<IDomainEvent> trong class AggregateRoot
    // => Tính chất đa hình
    // => Điểm hay của việc kế thừa từ interface
    public record ProductCreated(Guid IdEvent, Guid Id, string Name, decimal Price, string Description) : IDomainEvent, ICommand;

    public record ProductDeleted(Guid IdEvent, Guid Id) : IDomainEvent, ICommand;

    public record ProductUpdated(Guid IdEvent, Guid Id, string Name, decimal Price, string Description) : IDomainEvent, ICommand;
}
