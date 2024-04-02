using Microsoft.Extensions.DependencyInjection;
using Security.Objects;

namespace Security.Data.EF.MSSQL
{
    public static class SQLiteSecurityConfigurationExtensions
    {
        public static void AddMSSQLModelProvider(
            this SecurityConfiguration config,
            IServiceCollection services,
            string connectionString)
        {
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
}