using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace YounaSchool.Authuntication.Infrastructure.Persistence;

public sealed class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(GetConfigurationBasePath())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("Db")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? configuration.GetConnectionString("Db");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'AuthDb' was not found. " +
                "Checked appsettings.json, appsettings.Development.json, and environment variables.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
        optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.MigrationsAssembly(typeof(AuthDbContext).Assembly.FullName);
        });

        return new AuthDbContext(optionsBuilder.Options);
    }

    private static string GetConfigurationBasePath()
    {
        var currentDirectory = Directory.GetCurrentDirectory();

        var candidatePaths = new[]
        {
            currentDirectory,
            Path.Combine(currentDirectory, "..", "..", "..", "Host", "YouNaSchool.API", "YouNaSchool.API"),
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "..", "Host", "YouNaSchool.API", "YouNaSchool.API")
        };

        foreach (var candidatePath in candidatePaths)
        {
            var fullPath = Path.GetFullPath(candidatePath);
            if (File.Exists(Path.Combine(fullPath, "appsettings.json")))
            {
                return fullPath;
            }
        }

        throw new DirectoryNotFoundException(
            "Could not locate the API configuration directory containing appsettings.json.");
    }
}
