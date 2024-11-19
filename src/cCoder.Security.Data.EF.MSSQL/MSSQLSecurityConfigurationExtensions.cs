using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Security.Data.EF.MSSQL;

public static class MSSQLSecurityConfigurationExtensions
{
    public static void AddMSSQLModelProvider(
        this SecurityConfiguration config,
        IServiceCollection services,
        string connectionString)
    {
        services.AddScoped<ISecurityDbContextFactory>(sp => 
            new MSSQLSecurityDbContextFactory(connectionString)
            {
                GetAuthInfo = (withAuth) =>
                {
                    return withAuth
                        ? new SSOAuthInfo { SSOUserId = "Guest" } 
                        : sp.GetService<ISSOAuthInfo>();
                }
            });

        services.AddDistributedSqlServerCache(options =>
        {
            options.ConnectionString = connectionString;
            options.SchemaName = "dbo";
            options.TableName = "Sessions";
        });
    }
}