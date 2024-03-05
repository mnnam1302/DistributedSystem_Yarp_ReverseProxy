using System.Reflection;

namespace Authorization.Application
{
    public class AssemblyReference
    {
        public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
    }
}