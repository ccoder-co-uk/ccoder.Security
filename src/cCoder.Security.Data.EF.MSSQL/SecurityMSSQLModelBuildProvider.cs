using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq;

namespace cCoder.Security.Data.EF.MSSQL;

public partial class SecurityMSSQLModelBuildProvider : ISecurityModelBuildProvider
{
    readonly string connectionString;
    readonly bool logSQL;

    public SecurityMSSQLModelBuildProvider(string connectionString, bool logSQL = false)
    {
        this.connectionString = connectionString;
        this.logSQL = logSQL;
    }

    public void MigrateDatabase(DatabaseFacade database) => 
        database.Migrate();

    public void Create(ModelBuilder modelBuilder)
    {
        ConfigureSecurityModel(modelBuilder);

        var cascadingRelationships = modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (var relationship in cascadingRelationships)
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
    }

    public void Configure(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(connectionString, b => b.MigrationsAssembly("Security.Data.EF.MSSQL"));

        if (logSQL)
            optionsBuilder.LogTo((message) =>
            {
                if (message.Contains("Executing") || message.Contains("transaction"))
                    System.Diagnostics.Debug.WriteLine(message);
            });
    }
}