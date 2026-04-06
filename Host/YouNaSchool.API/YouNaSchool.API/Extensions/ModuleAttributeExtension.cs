using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Metadata;
using System.Reflection;

namespace YouNaSchool.API.Extensions;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ModuleAttribute : Attribute, IEndpointMetadataProvider
{
    public string Name { get; }

    public ModuleAttribute(string name)
    {
        Name = name;
    }

    public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
    {
        var attribute = method.DeclaringType?
            .GetCustomAttributes(typeof(ModuleAttribute), false)
            .FirstOrDefault() as ModuleAttribute;

        if (attribute is not null)
        {
            builder.Metadata.Add(attribute);
        }
    }
}