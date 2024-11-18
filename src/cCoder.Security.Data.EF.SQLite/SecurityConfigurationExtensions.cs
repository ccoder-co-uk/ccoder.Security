using cCoder.Security.Objects;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Security.Data.EF.SQLite;

public static class SQLiteSecurityConfigurationExtensions
{
    public static void AddSQLiteModelProvider(
        this SecurityConfiguration config,
        IServiceCollection services,
        string connectionString) =>
        services.AddSingleton<ISecurityModelBuildProvider>(new SecuritySQLiteModelBuildProvider(connectionString));
}