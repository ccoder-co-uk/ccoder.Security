// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Data.Brokers;
using cCoder.Security.Models;
using cCoder.Security.Models.Configurations;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Security.Data.EF;

public static class IServiceCollectionExtensions
{
    public static void AddSecurityData(
        this IServiceCollection services,
        Action<SecurityDataConfiguration> configure)
    {
        SecurityDataConfiguration configuration = new();
        configure?.Invoke(configuration);
        services.AddSecurityData(configuration);
    }

    public static void AddSecurityData(
        this IServiceCollection services,
        SecurityDataConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        services.AddDependencies(configuration);
    }

    private static void AddDependencies(
        this IServiceCollection services,
        SecurityDataConfiguration configuration)
    {
        services.AddTransient<ISecurityDbContextFactory>(
            implementationFactory: serviceProvider =>
            {
                MSSQLSecurityDbContextFactory contextFactory = new(
                    configuration: configuration);

                contextFactory.SetAuthInfoAccessor(
                    authInfoAccessor: ignoreAuthInfo =>
                        ignoreAuthInfo
                            ? new SSOAuthInfo { SSOUserId = "Guest" }
                            : serviceProvider.GetService<ISSOAuthInfo>());

                return contextFactory;
            });

        services.AddDistributedSqlServerCache(setupAction: options =>
        {
            options.ConnectionString = configuration.ConnectionString;
            options.SchemaName = "dbo";
            options.TableName = "Sessions";
        });
    }
}