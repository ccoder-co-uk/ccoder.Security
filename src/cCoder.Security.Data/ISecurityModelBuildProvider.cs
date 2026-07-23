// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace cCoder.Security.Data.EF;

public interface ISecurityModelBuildProvider
{
    void Configure(DbContextOptionsBuilder optionsBuilder);
    void Create(ModelBuilder newModelBuilder);
    void MigrateDatabase(DatabaseFacade database);
}