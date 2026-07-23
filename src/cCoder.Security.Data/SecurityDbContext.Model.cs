// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using Microsoft.EntityFrameworkCore;

namespace cCoder.Security.Data.EF;

public partial class SecurityDbContext
{
    public static void ConfigureSecurityModel(ModelBuilder modelBuilder)
    {
        ConfigureSecurityTables(modelBuilder: modelBuilder);
        ConfigureSecurityColumns(modelBuilder: modelBuilder);
        ConfigureSecurityForeignKeys(modelBuilder: modelBuilder);
    }

    public static void ConfigureSecurityTables(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SSORole>()
            .ToTable(name: "Roles");

        modelBuilder.Entity<SSOUser>()
            .ToTable(name: "Users");

        modelBuilder.Entity<SSOUserRole>()
            .ToTable(name: "UserRoles");

        modelBuilder.Entity<Tenant>()
            .ToTable(name: "Tenants");

        modelBuilder.Entity<TenantAnalysis>()
            .ToTable(name: "TenantAnalysis");

        modelBuilder.Entity<Token>()
            .ToTable(name: "Tokens");

        modelBuilder.Entity<UserEvent>()
            .ToTable(name: "UserEvents");
    }

    public static void ConfigureSecurityColumns(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SSORole>()
            .HasKey(keyExpression: ssoRole => ssoRole.Id);

        modelBuilder.Entity<SSORole>()
            .Property(propertyExpression: ssoRole => ssoRole.Name)
            .IsRequired();

        modelBuilder.Entity<SSOUser>()
            .HasKey(keyExpression: ssoUser => ssoUser.Id);

        modelBuilder.Entity<SSOUserRole>()
            .HasKey(keyExpression: ssoUserRole => new { ssoUserRole.RoleId, ssoUserRole.UserId });

        modelBuilder.Entity<Tenant>()
            .HasKey(keyExpression: t => t.Id);

        modelBuilder.Entity<Tenant>()
            .Property(propertyExpression: t => t.Id)
            .HasMaxLength(maxLength: 50);

        modelBuilder.Entity<Tenant>()
            .Property(propertyExpression: t => t.Name)
            .HasMaxLength(maxLength: 50)
            .IsRequired();

        modelBuilder.Entity<Tenant>()
            .Property(propertyExpression: t => t.Description)
            .HasMaxLength(maxLength: 500)
            .IsRequired();

        modelBuilder.Entity<Tenant>()
            .Property(propertyExpression: t => t.CreatedBy)
            .HasMaxLength(maxLength: 100);

        modelBuilder.Entity<Tenant>()
            .Property(propertyExpression: t => t.LastUpdatedBy)
            .HasMaxLength(maxLength: 100);

        modelBuilder.Entity<TenantAnalysis>()
            .HasKey(keyExpression: ta => ta.Id);

        modelBuilder.Entity<Token>()
            .HasKey(keyExpression: t => t.Id);

        modelBuilder.Entity<Token>()
            .Property(propertyExpression: t => t.Id)
            .HasMaxLength(maxLength: 64);

        modelBuilder.Entity<UserEvent>()
            .Property(propertyExpression: ue => ue.Id);
    }

    public static void ConfigureSecurityForeignKeys(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SSORole>()
            .HasMany(ssoRole => ssoRole.Users)
            .WithOne(navigationExpression: ssoUserRole => ssoUserRole.Role)
            .HasForeignKey(foreignKeyExpression: ssoUserRole => ssoUserRole.RoleId);

        modelBuilder.Entity<SSORole>()
            .HasOne(ssoRole => ssoRole.Tenant)
            .WithMany(navigationExpression: t => t.Roles)
            .HasForeignKey(foreignKeyExpression: ssoRole => ssoRole.TenantId);

        modelBuilder.Entity<SSOUser>()
            .HasMany(ssoUser => ssoUser.Roles)
            .WithOne(navigationExpression: ssoUserRole => ssoUserRole.User)
            .HasForeignKey(foreignKeyExpression: ssoUserRole => ssoUserRole.UserId);

        modelBuilder.Entity<SSOUser>()
            .HasMany(ssoUser => ssoUser.UserEvents)
            .WithOne(navigationExpression: userEvent => userEvent.CreatedByUser)
            .HasForeignKey(foreignKeyExpression: userEvent => userEvent.CreatedBy);

        modelBuilder.Entity<SSOUser>()
            .HasMany(ssoUser => ssoUser.Tokens)
            .WithOne(navigationExpression: token => token.User)
            .HasForeignKey(foreignKeyExpression: token => token.UserName);

        modelBuilder.Entity<SSOUserRole>()
            .HasOne(ssoUserRole => ssoUserRole.Role)
            .WithMany(navigationExpression: ssoRole => ssoRole.Users)
            .HasForeignKey(foreignKeyExpression: ssoUserRole => ssoUserRole.RoleId);

        modelBuilder.Entity<SSOUserRole>()
            .HasOne(ssoUserRole => ssoUserRole.User)
            .WithMany(navigationExpression: ssoUser => ssoUser.Roles)
            .HasForeignKey(foreignKeyExpression: ssoUserRole => ssoUserRole.UserId);

        modelBuilder.Entity<Tenant>()
            .HasMany(t => t.Roles)
            .WithOne(navigationExpression: ssoRole => ssoRole.Tenant)
            .HasForeignKey(foreignKeyExpression: ssoRole => ssoRole.TenantId);

        modelBuilder.Entity<Tenant>()
            .HasMany(t => t.UserEvents)
            .WithOne(navigationExpression: userEvent => userEvent.Tenant)
            .HasForeignKey(foreignKeyExpression: userEvent => userEvent.TenantId);

        modelBuilder.Entity<Tenant>()
            .HasMany(t => t.Analysis)
            .WithOne(navigationExpression: analysis => analysis.Tenant)
            .HasForeignKey(foreignKeyExpression: analysis => analysis.TenantId);

        modelBuilder.Entity<TenantAnalysis>()
            .HasOne(ta => ta.Tenant)
            .WithMany(navigationExpression: t => t.Analysis)
            .HasForeignKey(foreignKeyExpression: ta => ta.TenantId);

        modelBuilder.Entity<Token>()
            .HasOne(t => t.User)
            .WithMany(navigationExpression: u => u.Tokens)
            .HasForeignKey(foreignKeyExpression: t => t.UserName);

        modelBuilder.Entity<UserEvent>()
            .HasOne(ur => ur.Session)
            .WithMany(navigationExpression: s => s.UserEvents)
            .HasForeignKey(foreignKeyExpression: ur => ur.SessionId);

        modelBuilder.Entity<UserEvent>()
            .HasOne(ue => ue.Tenant)
            .WithMany(navigationExpression: t => t.UserEvents)
            .HasForeignKey(foreignKeyExpression: ue => ue.TenantId);

        modelBuilder.Entity<UserEvent>()
            .HasOne(ue => ue.CreatedByUser)
            .WithMany(navigationExpression: ssoUser => ssoUser.UserEvents)
            .HasForeignKey(foreignKeyExpression: ue => ue.CreatedBy);
    }
}
