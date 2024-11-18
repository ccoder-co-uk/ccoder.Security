using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace cCoder.Security.Data.EF.SQLite;

public partial class SecuritySQLiteModelBuildProvider : ISecurityModelBuildProvider
{
    private readonly string connectionString;

    public SecuritySQLiteModelBuildProvider(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public void MigrateDatabase(DatabaseFacade database)
        => database.Migrate();

    public void Create(ModelBuilder modelBuilder)
    {
        ConfigureSecurityModel(modelBuilder);

        IEnumerable<Microsoft.EntityFrameworkCore.Metadata.IMutableForeignKey> cascadingRelationships = modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (Microsoft.EntityFrameworkCore.Metadata.IMutableForeignKey relationship in cascadingRelationships)
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
    }

    public void Configure(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite(connectionString, b => b.MigrationsAssembly("Security.Data.EF.SQLite"));
}