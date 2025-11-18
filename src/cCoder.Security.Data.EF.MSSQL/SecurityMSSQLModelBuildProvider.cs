using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Reflection;

namespace cCoder.Security.Data.EF.MSSQL;

public partial class SecurityMSSQLModelBuildProvider(string connectionString, bool logSQL = false) 
    : ISecurityModelBuildProvider
{
    public void MigrateDatabase(DatabaseFacade database) => 
        database.Migrate();

    public void Create(ModelBuilder modelBuilder)
    {
        ConfigureSecurityModel(modelBuilder);

        IEnumerable<Microsoft.EntityFrameworkCore.Metadata.IMutableForeignKey> cascadingRelationships = modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (Microsoft.EntityFrameworkCore.Metadata.IMutableForeignKey relationship in cascadingRelationships)
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
    }

    public void Configure(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(connectionString, b => b.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name));

        if (logSQL)
            optionsBuilder.LogTo((message) =>
            {
                if (message.Contains("Executing") || message.Contains("transaction"))
                    System.Diagnostics.Debug.WriteLine(message);
            });
    }
}