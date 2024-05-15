using DistributedSystem.Contract.Enumerations;

namespace DistributedSystem.Contract.Extensions;

public static class SortOrderExtension
{
    public static SortOrder ConvertStringToSortOrder(string? sortOrder)
    {
        return !string.IsNullOrWhiteSpace(sortOrder)
            ? sortOrder.Trim().ToUpper().Equals("ASC")
            ? SortOrder.Ascending : SortOrder.Descending
            : SortOrder.Descending;
    }

    /// <summary>
    /// Format: "Id-Asc, Name-Asc, Age, Desc".
    /// </summary>
    /// <param name="sortOrder"></param>
    /// <returns></returns>
    public static IDictionary<string, SortOrder> ConvertStringToSortOrderV2(string? sortOrder)
    {
        var result = new Dictionary<string, SortOrder>();

        if (!string.IsNullOrWhiteSpace(sortOrder))
        {
            // More than one pair Id-Asc, Name-Asc, Age, Desc
            if (sortOrder.Trim().Split(",").Length > 0)
            {
                foreach (var item in sortOrder.Trim().Split(","))
                {
                    if (!item.Contains("-"))
                    {
                        throw new FormatException("Sort Condition should be follow bt format: Column1-ASC,Column2-DESC,...");
                    }

                    var properties = item.Trim().Split("-");

                    var key = properties[0];
                    var value = ConvertStringToSortOrder(properties[1]);

                    result.TryAdd(key, value);
                }
            }
            else
            {
                // One pair Id-Asc
                if (!sortOrder.Contains("-"))
                {
                    throw new FormatException("Sort Condition should be follow bt format: ColumnName-ASC");
                }

                var property = sortOrder.Trim().Split("-");
                result.TryAdd(property[0], ConvertStringToSortOrder(property[1]));
            }
        }

        return result;
    }
}
