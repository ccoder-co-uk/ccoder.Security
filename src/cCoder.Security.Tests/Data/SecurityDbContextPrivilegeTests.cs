// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.EF;
using cCoder.Security.Objects;
using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Data;

public partial class SecurityDbContextPrivilegeTests
{
    [Fact]
    public void ShouldExposeCompleteSerializableSecurityPrivilegeCatalogue()
    {
        // Given
        SecurityDbContext context = new(
            Mock.Of<ISSOAuthInfo>(),
            new DbContextOptionsBuilder<SecurityDbContext>().Options);

        string[] expectedPrivilegeIds =
        [
            "security_admin",
            "tenant_create",
            "tenant_read",
            "tenant_update",
            "tenant_delete",
            "tenant_admin",
            "tenantsecret_create",
            "tenantsecret_read",
            "tenantsecret_update",
            "tenantsecret_delete",
            "userrole_create",
            "userrole_read",
            "userrole_delete"
        ];

        // When
        SSOPrivilege[] privileges = [.. context.GetPrivileges()];

        // Then
        privileges.Select(selector: privilege => privilege.Id)
            .Should()
            .BeEquivalentTo(expectation: expectedPrivilegeIds);

        privileges.Should()
            .OnlyContain(predicate: privilege =>
            !string.IsNullOrWhiteSpace(value: privilege.Id)
            && !string.IsNullOrWhiteSpace(value: privilege.Type)
            && !string.IsNullOrWhiteSpace(value: privilege.Operation)
            && !string.IsNullOrWhiteSpace(value: privilege.Description));
    }
}