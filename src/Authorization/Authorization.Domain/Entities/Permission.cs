namespace Authorization.Domain.Entities
{
    public class Permission
    {
        public Guid RoleId { get; set; }
        public string FunctionId { get; set; }
        public string ActionId { get; set; }
    }
}