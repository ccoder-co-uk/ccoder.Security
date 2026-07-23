// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.EF;
using cCoder.Security.Objects;
using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Data;

public class SecurityDbContextPrivilegeTests
{
    [Fact]
    public void ShouldExposeCompleteSerializableSecurityPrivilegeCatalogue()
    {
        // given
        SecurityDbContext context = new(
            Mock.Of<ISSOAuthInfo>(),
            Mock.Of<ISecurityModelBuildProvider>());

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

        // when
        SSOPrivilege[] privileges = [.. context.GetPrivileges()];

        // then
        privileges.Select(privilege => privilege.Id)
            .Should().BeEquivalentTo(expectation: expectedPrivilegeIds);

        privileges.Should().OnlyContain(predicate: privilege =>
            !string.IsNullOrWhiteSpace(privilege.Id)
            && !string.IsNullOrWhiteSpace(privilege.Type)
            && !string.IsNullOrWhiteSpace(privilege.Operation)
            && !string.IsNullOrWhiteSpace(privilege.Description));
    }
}