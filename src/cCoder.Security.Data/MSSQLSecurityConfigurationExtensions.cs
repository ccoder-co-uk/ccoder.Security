// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Data.EF.Dependencies;
using cCoder.Security.Objects;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Security.Data.EF;

public static class MSSQLSecurityConfigurationExtensions
{
    public static void AddMSSQLModelProvider(
        this SecurityConfiguration config,
        IServiceCollection services,
        string connectionString)
    {
        services.AddTransient<ISecurityDbContextFactory>(implementationFactory: sp =>
            new MSSQLSecurityDbContextFactory(connectionString)
            {
                GetAuthInfo = (withAuth) =>
                {
                    return withAuth
                        ? new SSOAuthInfo { SSOUserId = "Guest" }
                        : sp.GetService<ISSOAuthInfo>();
                }
            });

        services.AddDistributedSqlServerCache(setupAction: options =>
        {
            options.ConnectionString = connectionString;
            options.SchemaName = "dbo";
            options.TableName = "Sessions";
        });
    }
}