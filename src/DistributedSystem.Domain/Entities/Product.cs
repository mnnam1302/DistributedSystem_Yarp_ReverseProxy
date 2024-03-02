using DistributedSystem.Domain.Abstractions.Aggregates;
using DistributedSystem.Domain.Abstractions.Entities;
using System.Diagnostics.Contracts;
using static DistributedSystem.Domain.Exceptions.ProductException;

namespace DistributedSystem.Domain.Entities
{
    public class Product : AggregateRoot<Guid>, IAuditableEntity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public DateTimeOffset CreatedOnUtc { get; set; }
        public DateTimeOffset? ModifiedOnUtc { get; set; }

        public Product(Guid id, string name, decimal price, string description)
        {
            // Check lỗi ở đây, nếu có throw ra luôn => Domain driven design
            // Tất cả code nằm chung một chỗ, sau này một thành viên nào không đọc kỹ về requirement, thì sẽ bị lỗi
            // Code centralized
            // Khi throw ra thì phải hướng nó ở Global Exception Handler => Hướng đúng lỗi => ExceptionHandlingMiddleware

            // Refactor code => Đặt ở đây không hợp lý => Bản chất constructor chỉ nhận tham số và truyền vào những cái fields
            //if (name.Length > 10)
            //    throw new ProductFieldException(nameof(Name));

            Id = id;
            Name = name;
            Price = price;
            Description = description;
        }

        // Thằng này mới nên chưa có instance nào reference đến Create, ví dụ, instance.Create(...) => Không có => có static
        public static Product Create(Guid id, string name, decimal price, string description)
        {
            if (name.Length > 50)
                throw new ProductFieldException(nameof(Name));

            var product = new Product(id, name, price, description);

            // Khi Raise event , thì sẽ add class ProductCreated vào trong List
            product.RaiseDomainEvent(new Contract.Services.V1.Product.DomainEvent.ProductCreated(Guid.NewGuid(), product.Id,
                product.Name, product.Price, 
                product.Description
                ));

            return product;
        }

        // Có instance hay đối tượng reference đến Update, ví dụ, instance.Update(...) 
        // Phải tham chiếu từ một đối tượng cụ thể
        public void Update(string name, decimal price, string description)
        {
            if (name.Length > 50)
                throw new ProductFieldException(nameof(Name));

            Name = name;
            Price = price;
            Description = description;

            RaiseDomainEvent(new Contract.Services.V1.Product.DomainEvent.ProductUpdated(Guid.NewGuid(), Id, Name, Price, Description));
        }

        // Có instance hay đối tượng reference đến Update, ví dụ, instance.Update(...)
        public void Delete()
            => RaiseDomainEvent(new Contract.Services.V1.Product.DomainEvent.ProductDeleted(Guid.NewGuid(), Id));
    }
}