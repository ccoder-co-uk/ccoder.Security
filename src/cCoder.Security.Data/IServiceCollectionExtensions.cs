// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Data.Brokers;
using cCoder.Security.Data.Dependencies.Caching;
using cCoder.Security.Models;
using cCoder.Security.Models.Configurations;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.SqlServer;
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

        services.AddMemoryCache();

        services.Configure<SqlServerCacheOptions>(configureOptions: options =>
        {
            options.ConnectionString = configuration.ConnectionString;
            options.SchemaName = "dbo";
            options.TableName = "Sessions";
        });

        services.AddSingleton<SqlServerCache>();

        services.AddSingleton<IDistributedCache>(
            implementationFactory: serviceProvider =>
                new LocallyCachedDistributedCache(
                    distributedCache: serviceProvider
                        .GetRequiredService<SqlServerCache>(),
                    memoryCache: serviceProvider
                        .GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>()));
    }
}