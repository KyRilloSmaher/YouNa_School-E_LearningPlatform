using Auth.Infrastructure.Messaging;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using SharedKernel.Infrastructure.Messaging.RabbitMQ;
using System.Threading.RateLimiting;
using YounaSchool.Authuntication.Application.IntegrationEvents;
using Users.Infrastructure.Messaging;
using Wallet.Infrastructure.Messaging;
using YounaSchool.Authuntication.Application;
using YounaSchool.Authuntication.Infrastructure;
using YounaSchool.Authuntication.Infrastructure.Services;
using YouNaSchool.API;
using YouNaSchool.API.Middlewares;
using YouNaSchool.Notifications.Infrastructure;
using YouNaSchool.Notifications.Infrastructure.Messaging;
using YouNaSchool.Notifications.Application.IntegrationEvents;
using YouNaSchool.Notifications.Application.IntegrationEvents.Handlers;
using Shared.Application.IntegrationEvents;
using YouNaSchhol.Users.Application;
using YouNaSchool.Users.Infrastructure;
using YouNaSchool.Wallet.Application;
using YouNaSchool.Wallet.Application.Abstractions.OtherModules;
using YouNaSchool.Wallet.Application.Messaging;
using YouNaSchool.Wallet.Application.Validators;

var builder = WebApplication.CreateBuilder(args);

#region -------------------- Serilog Logging --------------------
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Map(
        keyPropertyName: "Module",
        defaultKey: "General",
        configure: (module, wt) =>
        {
            wt.File(
                path: $"Logs/{module}/log-.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                outputTemplate:
                    "[{Timestamp:yyyy-MM-dd HH:mm:ss} | {Level:u3}] " +
                    "Module: {Module} | TraceId: {TraceId} | " +
                    "{Message:lj}{NewLine}{Exception}{NewLine}");
        })
    .CreateLogger();

builder.Host.UseSerilog()
       .ConfigureHostOptions(opt =>
       {
           // Prevent host from stopping on background service exceptions
           opt.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
       });
#endregion

#region -------------------- Core Services --------------------
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<PayLectureCommandValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
builder.Services.AddHttpClient();
#endregion

#region -------------------- Hangfire --------------------
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(
        builder.Configuration.GetConnectionString("HangfireConnection"),
        new SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.Zero,
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true
        }));

builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = 5;
    options.ServerName = "YouNaSchool-Server";
});
#endregion

#region -------------------- Dependency Injection --------------------
// Infrastructure first (DB, Repositories, External services)
builder.Services.AddWalletInfrastructure(builder.Configuration); // MUST be first for Outbox repo
builder.Services.AddUsersInfrastructure(builder.Configuration);
builder.Services.AddAuthInfrastructure(builder.Configuration);
builder.Services.AddNotificationInfrastructure(builder.Configuration);

// Application layer (MediatR handlers, Validators)
builder.Services.AddWalletApplication();
builder.Services.AddAuthApplication();
YouNaSchhol.Users.Application.DependencyInjection.AddApplication(builder.Services);

builder.Services.AddScoped<IAuthUserProvider, AuthUserProvider>();
#endregion

#region -------------------- Messaging per module --------------------
builder.Services.AddUsersMessaging(builder.Configuration);
builder.Services.AddAuthMessaging(builder.Configuration);
builder.Services.AddWalletMessaging(builder.Configuration);
builder.Services.AddNotificationsMessaging(builder.Configuration); // Correct PascalCase
builder.Services.AddScoped<IIntegrationEventHandler<SendConfirmationEmailIntegrationEvent>, SendConfirmationEmailHandler>();
builder.Services.AddScoped<IIntegrationEventHandler<UserCreatedIntegrationEvent>, CreateDefaultNotificationPreferencesHandler>();
builder.Services.AddScoped<IIntegrationEventHandler<LecturePaidIntegrationEvent>, SendLecturePaidNotificationHandler>();
builder.Services.AddScoped<IIntegrationEventHandler<WalletRechargedIntegrationEvent>, SendWalletRechargeNotificationHandler>();
#endregion

#region -------------------- API Versioning --------------------
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
#endregion

#region -------------------- CORS --------------------
const string CORS = "_DefaultCors";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CORS, policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowAnyOrigin();
    });
});
#endregion

#region -------------------- Rate Limiting --------------------
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("DefaultPolicy", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});
#endregion

var app = builder.Build();

#region -------------------- Initialize RabbitMQ topology before consumers --------------------
await app.InitializeRabbitMqTopologyAsync(UsersMessagingConfiguration.BuildTopology());
await app.InitializeRabbitMqTopologyAsync(AuthMessagingConfiguration.BuildTopology());
await app.InitializeRabbitMqTopologyAsync(WalletMessagingConfiguration.BuildTopology());
await app.InitializeRabbitMqTopologyAsync(NotificationsMessagingConfiguration.BuildTopology());
#endregion

#region -------------------- Middleware pipeline --------------------
app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint(
            $"/swagger/{description.GroupName}/swagger.json",
            description.GroupName.ToUpperInvariant());
    }
});

app.UseCors(CORS);

app.UseRateLimiter();

app.UseMiddleware<ModuleLoggingMiddleware>();  // Enrich logs with Module
app.UseMiddleware<ErrorHandlerMiddleware>();   // Global exception handler

app.UseAuthentication();
app.UseAuthorization();

// Add Hangfire Dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() }
});
#endregion
app.MapControllers();
app.Run();

#region -------------------- Hangfire Authorization Filter --------------------
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context) => true;
}
#endregion
