using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shared.Application.ClockandUserContext;
using SharedKernel.Application.ClockandUserContext;
using SharedKernel.Application.Messaging.Outbox;
using SharedKernel.Application.UNIT_OF_WORK;
using SharedKernel.Infrastructure.Messaging.RabbitMQ;
using Wallet.Infrastructure.Persistence;
using YouNaSchool.Wallet.Application.Abstractions.ExternalService;
using YouNaSchool.Wallet.Application.Abstractions.ExternalService.Payment;
using YouNaSchool.Wallet.Application.Abstractions.Messaging;
using YouNaSchool.Wallet.Application.Abstractions.Persistence;
using YouNaSchool.Wallet.Application.Messaging;
using YouNaSchool.Wallet.Domain.Repositories;
using YouNaSchool.Wallet.Infrastructure.ExternalService;
using YouNaSchool.Wallet.Infrastructure.ExternalService.Payments;
using YouNaSchool.Wallet.Infrastructure.ExternalService.Payments.PayPal;
using YouNaSchool.Wallet.Infrastructure.ExternalService.Payments.Stripe;
using YouNaSchool.Wallet.Infrastructure.IntegrationEvents.Handlers;
using YouNaSchool.Wallet.Infrastructure.Persistence;
using YouNaSchool.Wallet.Infrastructure.Persistence.Repositories;
using YouNaSchool.Wallet.Infrastructure.Repositories;
using YouNaSchool.Wallet.Infrastructure.Settings;
using YouNaSchhol.Users.Application.IntegrationEvents;

public static class DependencyInjection
{
    public static IServiceCollection AddWalletInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        #region Connection To SQL SERVER

        var connectionString = configuration.GetConnectionString("Db");

        services.AddDbContext<WalletDbContext>(options =>
        {
            options.UseSqlServer(
                connectionString
                );
        });

        #endregion

        #region HANGFIRE
        services.AddHangfire(config =>
        {
            config.UseSqlServerStorage(connectionString);
        });

        services.AddHangfireServer();
        #endregion

        #region UNIT OF WORK
        services.AddSingleton<ISystemClock, SystemClock>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        #endregion

        #region REPOSITORIES
        services.AddScoped<IWalletRepository, WalletRepository>();
        services.AddScoped<IWalletRechargeRepository, WalletRechargeRepository>();
        services.AddScoped<IWalletLedgerEntryRepository, WalletLedgerEntryyRepository>();
        services.AddScoped<IOutBoxMessageRepository, OutboxMessageRepository>();
        #endregion

        services.AddRabbitMqInfrastructure(configuration);

        services.AddScoped<IIntegrationEventHandler<StudentCreatedIntegrationEvent>, StudentRegisteredIntegrationEventHandler>();

        #region STRIPE
        services.AddScoped<IPaymentGatewayFactory, PaymentGatewayFactory>();
        services.Configure<StripeSettings>(configuration.GetSection("StripeSettings"));
        services.AddSingleton<IPaymentService, StripeService>();
        #endregion

        #region PAYPAL
        services.Configure<PayPalSettings>(configuration.GetSection("PayPal"));
        services.AddHttpClient<PayPalPaymentGateway>(client =>
        {
            client.BaseAddress = new Uri(configuration["PayPal:BaseUrl"]!);
        });

        services.AddScoped<PayPalAuthService>();
        services.AddScoped<IPaymentService, PayPalPaymentGateway>();
        #endregion

        #region EMAIL
        services.Configure<EmailSettings>(configuration.GetSection("Email"));
        //services.AddScoped<IEmailService, EmailService>();
        #endregion

        return services;
    }
}
