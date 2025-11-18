using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace cCoder.Security.Data.EF.SQLite;

public partial class SecuritySQLiteModelBuildProvider(string connectionString) 
    : ISecurityModelBuildProvider
{
    public void MigrateDatabase(DatabaseFacade database) => 
        database.Migrate();

    public void Create(ModelBuilder modelBuilder)
    {
        ConfigureSecurityModel(modelBuilder);

        IEnumerable<IMutableForeignKey> cascadingRelationships = 
            modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (IMutableForeignKey relationship in cascadingRelationships)
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
    }

    public void Configure(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite(
            connectionString, 
            b => b.MigrationsAssembly("Security.Data.EF.SQLite"));
}