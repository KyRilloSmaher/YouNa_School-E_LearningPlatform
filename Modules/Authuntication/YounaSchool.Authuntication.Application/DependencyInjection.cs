using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using YounaSchool.Authuntication.Application.Behaviors;
using YounaSchool.Authuntication.Application.Mapping;

namespace YounaSchool.Authuntication.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddAutoMapper(typeof(AuthMappingProfile).Assembly);
        services.AddValidatorsFromAssembly(assembly);

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(AuthValidationBehavior<,>));

        return services;
    }
}
