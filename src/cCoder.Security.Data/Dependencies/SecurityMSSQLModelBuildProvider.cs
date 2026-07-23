// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Reflection;

namespace cCoder.Security.Data.EF.Dependencies;

public partial class SecurityMSSQLModelBuildProvider(string connectionString, bool logSQL = false)
    : ISecurityModelBuildProvider
{
    public void MigrateDatabase(DatabaseFacade database) =>
        database.Migrate();

    public void Create(ModelBuilder modelBuilder)
    {
        ConfigureSecurityModel(modelBuilder: modelBuilder);

        IEnumerable<Microsoft.EntityFrameworkCore.Metadata.IMutableForeignKey> cascadingRelationships = modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Where(predicate: fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (Microsoft.EntityFrameworkCore.Metadata.IMutableForeignKey relationship in cascadingRelationships)
        { relationship.DeleteBehavior = DeleteBehavior.Restrict; }
    }

    public void Configure(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(connectionString: connectionString, sqlServerOptionsAction: b => b.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name));

        if (logSQL)
        {
            optionsBuilder.LogTo(action: (message) =>
        {
            if (message.Contains("Executing") || message.Contains("transaction"))
            { System.Diagnostics.Debug.WriteLine(message); }
        });
        }
    }
}