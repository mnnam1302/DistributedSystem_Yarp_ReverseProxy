namespace Authorization.Domain.Entities;

public class Function
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string Url { get; set; }

    public string ParentId { get; set; }

    public int? SortOrder { get; set; }

    public string CssClass { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Permission> Permissions { get; set; }

    public virtual ICollection<ActionInFunction> ActionInFunctions { get; set; }
}
