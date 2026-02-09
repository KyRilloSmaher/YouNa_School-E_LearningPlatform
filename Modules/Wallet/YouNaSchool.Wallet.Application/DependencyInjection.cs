using Microsoft.Extensions.DependencyInjection;
using YouNaSchool.Wallet.Application.Mapping;

namespace YouNaSchool.Wallet.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddWalletApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;
        services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));
        services.AddAutoMapper(typeof(WalletMappingProfile).Assembly);
        return services;
    }

}
