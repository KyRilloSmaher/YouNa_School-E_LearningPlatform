using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shared.Application.ClockandUserContext;
using SharedKernel.Application.ClockandUserContext;
using SharedKernel.Application.UNIT_OF_WORK;
using Wallet.Infrastructure.Persistence;
using Wallet.Infrastructure.Persistence.UnitOfWork;
using YouNaSchool.Wallet.Application.Abstractions.ExternalService;
using YouNaSchool.Wallet.Application.Abstractions.ExternalService.Payment;
using YouNaSchool.Wallet.Application.Abstractions.Messaging;
using YouNaSchool.Wallet.Application.Abstractions.Persistence;
using YouNaSchool.Wallet.Application.IntegrationEvents;
using YouNaSchool.Wallet.Domain.Repositories;
using YouNaSchool.Wallet.Infrastructure.ExternalService;
using YouNaSchool.Wallet.Infrastructure.ExternalService.Payments;
using YouNaSchool.Wallet.Infrastructure.ExternalService.Payments.PayPal;
using YouNaSchool.Wallet.Infrastructure.ExternalService.Payments.Stripe;
using YouNaSchool.Wallet.Infrastructure.IntegrationEvents.Handlers;
using YouNaSchool.Wallet.Infrastructure.Messaging.Outbox;
using YouNaSchool.Wallet.Infrastructure.Messaging.RabbitMQ;
using YouNaSchool.Wallet.Infrastructure.Persistence.Repositories;
using YouNaSchool.Wallet.Infrastructure.Repositories;
using YouNaSchool.Wallet.Infrastructure.Settings;

public static class DependencyInjection
{
    public static IServiceCollection AddWalletInfrastructure(this IServiceCollection services,IConfiguration configuration)
    {
       
        #region Connection To SQL SERVER

        var connectionString = configuration.GetConnectionString("WalletDb");

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
        services.AddScoped<IOutBoxMessageRepository, OutBoxMessageRepository>();
        #endregion

        #region OUTBOX
        services.AddScoped<IOutboxJob, OutboxJob>();
        services.AddScoped<IOutBoxProcessor, OutBoxProcessor>();
        #endregion

        #region RABBITMQ
        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMQ"));
        services.AddSingleton(sp =>sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value);
        services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();
        services.AddSingleton<IRabbitMqConsumer, RabbitMqConsumer>();
        #endregion

        #region INTEGRATION EVENTS
        services.AddSingleton<IIntegrationEventHandler, StudentRegistrationIntegrationEventHandler>();
        services.AddSingleton<IIntegrationEventHandler, PaymentCompletedIntegrationEventHandler>();
        #endregion

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
        services.AddScoped<IEmailService, EmailService>();
        #endregion
        return services;
    }
}