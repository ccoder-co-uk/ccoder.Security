// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects;

namespace cCoder.Security.Data.EF.Dependencies;

public class MSSQLSecurityDbContextFactory()
    : ISecurityDbContextFactory
{
    private readonly string connectionString;

    public Func<bool, ISSOAuthInfo> GetAuthInfo { get; set; } =
        (x) => new SSOAuthInfo { SSOUserId = "Guest" };

    public MSSQLSecurityDbContextFactory(string connectionString) : this() =>
        this.connectionString = connectionString;

    public SecurityDbContext CreateDbContext(bool ignoreAuthInfo = false) =>
        new(GetAuthInfo(arg: ignoreAuthInfo), new SecurityMSSQLModelBuildProvider(connectionString ?? "SSO"));

    public SecurityDbContext CreateDbContext(string[] args) =>
        CreateDbContext();
}