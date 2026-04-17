using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Security.Data.EF.SQLite;

public static class SQLiteSecurityConfigurationExtensions
{
    public static void AddSQLiteModelProvider(
        this SecurityConfiguration config,
        IServiceCollection services,
        string connectionString)
    {
        services.AddScoped<ISecurityDbContextFactory>(sp =>
            new SQLiteSecurityDbContextFactory(connectionString)
            {
                GetAuthInfo = withAuth =>
                    withAuth
                        ? new SSOAuthInfo { SSOUserId = "Guest" }
                        : sp.GetService<ISSOAuthInfo>()
            });

        services.AddDistributedMemoryCache();
        services.AddSingleton<ISecurityModelBuildProvider>(new SecuritySQLiteModelBuildProvider(connectionString));
    }
}
