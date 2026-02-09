using Hangfire;
using YouNaSchool.Wallet.Application;
using YouNaSchool.Wallet.Infrastructure.Messaging.Outbox;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

#region Swagger_Gn
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo { Title = "YouNa School E-Learning Platform Wallet Module ", Version = "v1" });
//    c.EnableAnnotations();

//    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
//    {
//        Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
//        Name = "Authorization",
//        In = ParameterLocation.Header,
//        Type = SecuritySchemeType.ApiKey,
//        Scheme = JwtBearerDefaults.AuthenticationScheme

//    });

//    c.AddSecurityRequirement(new OpenApiSecurityRequirement
//                    {
//                     {
//                     new OpenApiSecurityScheme
//                     {
//                         Reference = new OpenApiReference
//                         {
//                             Type = ReferenceType.SecurityScheme,
//                             Id = JwtBearerDefaults.AuthenticationScheme
//                         }
//                     },
//                     Array.Empty<string>()
//                     }
//                   });
//});
#endregion

#region Dependency Injection
builder.Services.AddWalletInfrastructure(builder.Configuration).AddWalletApplication();
#endregion

#region AllowCORS
var CORS = "_DefaultCors";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CORS,
                      policy =>
                      {
                          policy.AllowAnyHeader();
                          policy.AllowAnyMethod();
                          policy.AllowAnyOrigin();
                      });
});

#endregion
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();



app.UseSwagger();
app.UseSwaggerUI();
app.UseHangfireDashboard("/hangfire");
RecurringJob.AddOrUpdate<IOutboxJob>(
    "wallet-outbox-processor",
    job => job.ExecuteAsync(),
    Cron.Minutely);
//app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors(CORS);

app.MapControllers();

app.Run();



