// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace cCoder.Security.Data.EF.Dependencies;

public class MSSQLSecurityDbContextFactory()
    : ISecurityDbContextFactory
{
    private readonly string connectionString;

    public Func<bool, ISSOAuthInfo> GetAuthInfo { get; set; } =
        (x) => new SSOAuthInfo { SSOUserId = "Guest" };

    public MSSQLSecurityDbContextFactory(string connectionString) : this() =>
        this.connectionString = connectionString;

    public SecurityDbContext CreateDbContext(bool ignoreAuthInfo = false)
    {
        DbContextOptionsBuilder<SecurityDbContext> optionsBuilder = new();

        optionsBuilder.UseSqlServer(
            connectionString: connectionString ?? "SSO",
            sqlServerOptionsAction: options => options.MigrationsAssembly(
                assemblyName: Assembly
                    .GetExecutingAssembly()
                    .GetName()
                    .Name));

        return new SecurityDbContext(
            authInfo: GetAuthInfo(arg: ignoreAuthInfo),
            options: optionsBuilder.Options);
    }

    public SecurityDbContext CreateDbContext(string[] args) =>
        CreateDbContext();
}