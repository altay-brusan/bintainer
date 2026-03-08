using System.Reflection;

namespace Bintainer.Modules.Reports.Application;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
