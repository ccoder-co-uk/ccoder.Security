using Microsoft.Extensions.DependencyInjection;
using cCoder.Security.Data.EF.MSSQL;
using cCoder.Security.Objects;

namespace cCoder.Security.Data.EF.SQLite
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