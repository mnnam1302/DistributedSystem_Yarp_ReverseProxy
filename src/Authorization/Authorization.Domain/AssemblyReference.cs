using System.Reflection;

namespace Authorization.Domain
{
    public class AssemblyReference
    {
        public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
    }
}