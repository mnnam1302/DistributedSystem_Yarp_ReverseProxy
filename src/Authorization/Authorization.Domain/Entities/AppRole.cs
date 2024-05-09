using Microsoft.AspNetCore.Identity;

namespace Authorization.Domain.Entities;

public class AppRole : IdentityRole<Guid>
{
    public string Description { get; set; }

    public string RoleCode { get; set; }

    public virtual ICollection<IdentityUserRole<Guid>> UserRoles { get; set; }

    public virtual ICollection<IdentityRoleClaim<Guid>> Claims { get; set; }

    public virtual ICollection<Permission> Permissions { get; set; }
}
