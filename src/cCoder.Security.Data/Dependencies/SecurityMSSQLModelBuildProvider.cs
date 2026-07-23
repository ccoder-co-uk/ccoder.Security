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

    public void Create(ModelBuilder newModelBuilder)
    {
        ConfigureSecurityModel(modelBuilder: newModelBuilder);

        IEnumerable<Microsoft.EntityFrameworkCore.Metadata.IMutableForeignKey> cascadingRelationships = newModelBuilder.Model.GetEntityTypes()
            .SelectMany(selector: t => t.GetForeignKeys())
            .Where(predicate: fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (Microsoft.EntityFrameworkCore.Metadata.IMutableForeignKey relationship in cascadingRelationships)
        { relationship.DeleteBehavior = DeleteBehavior.Restrict; }
    }

    public void Configure(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(connectionString: connectionString, sqlServerOptionsAction: b => b.MigrationsAssembly(assemblyName: Assembly.GetExecutingAssembly().GetName().Name));

        if (logSQL)
        {
            optionsBuilder.LogTo(action: (message) =>
        {
            if (message.Contains(value: "Executing") || message.Contains(value: "transaction"))
            { System.Diagnostics.Debug.WriteLine(message: message); }
        });
        }
    }
}