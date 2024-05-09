namespace DistributedSystem.Domain.Abstractions.Primitives;

public abstract class ValueObject : IEquatable<ValueObject>
{
    public abstract IEnumerable<object> GetAtomicValues();

    // Compare with other value object
    public bool Equals(ValueObject? other) => other is not null && ValuesAreEqual(other);

    // Compare with other object
    public override bool Equals(object? obj) => obj is ValueObject other && ValuesAreEqual(other);

    // Đảm bảo rằng phương thức GetHashCode trả về cùng một giá trị cho các đối tượng có cùng giá trị nguyên tử. Điều này là quan trọng để đảm bảo tính nhất quán khi sử dụng các đối tượng trong các cấu trúc dữ liệu như bảng băm.
    public override int GetHashCode() =>
        GetAtomicValues()
            .Aggregate(
                default(int),
                HashCode.Combine);

    private bool ValuesAreEqual(ValueObject other) => GetAtomicValues().SequenceEqual(other.GetAtomicValues());
}
