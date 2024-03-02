using System.Net;

namespace DistributedSystem.Domain.Abstractions.Primitives
{
    public abstract class ValueObject : IEquatable<ValueObject>
    {
        public abstract IEnumerable<object> GetAtomicValues();

        // Compare with other value object
        public bool Equals(ValueObject? other)
        {
            return other is not null && ValuesAreEqual(other);
        }

        // Compare with other object
        public override bool Equals(object? obj)
        {
            return obj is ValueObject other && ValuesAreEqual(other);
        }

        // Đảm bảo rằng phương thức GetHashCode trả về cùng một giá trị cho các đối tượng có cùng giá trị nguyên tử. Điều này là quan trọng để đảm bảo tính nhất quán khi sử dụng các đối tượng trong các cấu trúc dữ liệu như bảng băm.
        public override int GetHashCode()
        {
            return GetAtomicValues()
                .Aggregate(
                    default(int),
                    HashCode.Combine);
        }

        private bool ValuesAreEqual(ValueObject other)
        {
            return GetAtomicValues().SequenceEqual(other.GetAtomicValues());
        }
    }
}