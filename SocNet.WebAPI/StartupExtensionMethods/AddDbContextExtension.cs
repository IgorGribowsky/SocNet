using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.EntityFrameworkCore;
using SocNet.Infrastructure.EFRepository;

namespace SocNet.WebAPI.StartupExtensionMethods;

public static class AddDbContextExtension
{
    public static void AddDbContextBasedOnDbTypeEnvVar(this IServiceCollection services)
    {
        var dbType = Environment.GetEnvironmentVariable("DB_TYPE");
        if (string.IsNullOrEmpty(dbType))
            throw new Exception("Can't find DB_TYPE env var");

        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        if (string.IsNullOrEmpty(connectionString))
            throw new Exception("Can't find CONNECTION_STRING env var");

        switch (dbType)
        {
            case "MSSQL":
                services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connectionString));
                break;

            case "POSTGRE":
                services.AddEntityFrameworkNpgsql()
                .AddDbContext<ApplicationContext>
                (options =>
                options.UseNpgsql(connectionString));
                break;

            default:
                throw new Exception("DB_TYPE env var not recognized");
        }
    }
}
