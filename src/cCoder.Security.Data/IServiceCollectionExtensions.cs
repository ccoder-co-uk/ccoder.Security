// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.EF.Dependencies;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Security.Data.EF;

public static class IServiceCollectionExtensions
{
    public static void AddSecurityData(
        this IServiceCollection services,
        Action<SecurityConfiguration> configure)
    {
        SecurityConfiguration configuration = new();
        configure?.Invoke(configuration);
        services.AddSecurityData(configuration);
    }

    public static void AddSecurityData(
        this IServiceCollection services,
        SecurityConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        services.AddDependencies(configuration);
    }

    private static void AddDependencies(
        this IServiceCollection services,
        SecurityConfiguration configuration)
    {
        services.AddTransient<ISecurityDbContextFactory>(
            implementationFactory: serviceProvider =>
                new MSSQLSecurityDbContextFactory(
                    configuration.ConnectionString)
                {
                    GetAuthInfo = withAuth =>
                        withAuth
                            ? new SSOAuthInfo { SSOUserId = "Guest" }
                            : serviceProvider.GetService<ISSOAuthInfo>(),
                });

        services.AddDistributedSqlServerCache(setupAction: options =>
        {
            options.ConnectionString = configuration.ConnectionString;
            options.SchemaName = "dbo";
            options.TableName = "Sessions";
        });
    }
}
