using Microsoft.Extensions.DependencyInjection;
using Security.Data.EF.MSSQL;
using Security.Objects;

namespace Security.Data.EF.SQLite
{
    public static class SQLiteSecurityConfigurationExtensions
    {
        public static void AddSQLiteModelProvider(
            this SecurityConfiguration config,
            IServiceCollection services,
            string connectionString) =>
            services.AddSingleton<ISecurityModelBuildProvider>(new SecuritySQLiteModelBuildProvider(connectionString));
    }
}