// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Data.EF;
using cCoder.Security.Models;
using cCoder.Security.Models.Configurations;
using Microsoft.EntityFrameworkCore;

namespace cCoder.Security.Data.Brokers;

internal sealed class MSSQLSecurityDbContextFactory(
    SecurityDataConfiguration configuration)
    : ISecurityDbContextFactory
{
    private Func<bool, ISSOAuthInfo> getAuthInfo =
        _ => new SSOAuthInfo { SSOUserId = "Guest" };

    public MSSQLSecurityDbContextFactory()
        : this(
            configuration: new SecurityDataConfiguration
            {
                ConnectionString = "SSO"
            })
    {
    }

    public SecurityDbContext CreateDbContext(bool ignoreAuthInfo = false)
    {
        return new SecurityDbContext(
            authInfo: getAuthInfo(arg: ignoreAuthInfo),
            options: CreateOptions());
    }

    public SecurityDbContext CreateDbContext(string[] args) =>
        CreateDbContext(ignoreAuthInfo: true);

    internal void SetAuthInfoAccessor(
        Func<bool, ISSOAuthInfo> authInfoAccessor) =>
        getAuthInfo = authInfoAccessor;

    private DbContextOptions<SecurityDbContext> CreateOptions()
    {
        DbContextOptionsBuilder<SecurityDbContext> optionsBuilder = new();

        optionsBuilder.UseSqlServer(
            connectionString: configuration.ConnectionString ?? "SSO",
            sqlServerOptionsAction: options =>
                options.MigrationsAssembly(
                    assemblyName: "cCoder.Security.Data"));

        return optionsBuilder.Options;
    }
}