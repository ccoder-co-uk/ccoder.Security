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
            new MSSQLSecurityDbContextFactory()
            {
                GetAuthInfo = () =>
                {
                    try
                    {
                        return sp.GetService<ISSOAuthInfo>();
                    }
                    catch
                    {
                        return new SSOAuthInfo { SSOUserId = "Guest" };
                    }
                }
            });

        services.AddDbContextFactory<SecurityDbContext>();

        services.AddSingleton<ISecurityModelBuildProvider>(
            new SecurityMSSQLModelBuildProvider(connectionString));

        services.AddDistributedSqlServerCache(options =>
        {
            options.ConnectionString = connectionString;
            options.SchemaName = "dbo";
            options.TableName = "Sessions";
        });
    }
}