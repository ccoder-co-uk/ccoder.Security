using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Services.Tests.Orchestration;

public partial class SSOUserRegistrationOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldRegisterFirstUserAndAttachBootstrapTenantAdministratorRole()
    {
        RegisterUser input = new()
        {
            DisplayName = "Setup Admin",
            Email = "setup.admin@example.com",
            Password = "Password1!",
            PhoneNumber = "123",
            TenantId = "tenant-1"
        };

        SSOUser storedUser = new()
        {
            Id = "setup.admin",
            DisplayName = input.DisplayName,
            Email = input.Email
        };

        SSORole bootstrapRole = new()
        {
            Id = Guid.NewGuid(),
            TenantId = "tenant-1",
            UsersArePortalAdmins = true
        };

        ssoUserProcessingServiceMock
            .Setup(x => x.RegisterSSOUserAsync(It.IsAny<SSOUser>()))
            .ReturnsAsync(storedUser);
        userRoleProcessingServiceMock
            .Setup(x => x.GetAllSSOUserRoles())
            .Returns(Array.Empty<SSOUserRole>().AsQueryable());
        roleProcessingServiceMock
            .Setup(x => x.GetAllSSORoles(true))
            .Returns(new[] { bootstrapRole }.AsQueryable());
        userRoleOrchestrationServiceMock
            .Setup(x => x.AddSSOUserRoleAsync(It.IsAny<SSOUserRole>()))
            .ReturnsAsync((SSOUserRole userRole) => userRole);
        tokenProcessingServiceMock
            .Setup(x => x.GenerateConfirmationToken(storedUser.Id))
            .ReturnsAsync(new Token { Id = "token-1" });

        (SSOUser actualUser, string token) =
            await ssoUserRegistrationOrchestrationService.Register(input);

        actualUser.Should().BeSameAs(storedUser);
        token.Should().Be("token-1");
        userRoleOrchestrationServiceMock.Verify(
            x => x.AddSSOUserRoleAsync(It.Is<SSOUserRole>(userRole =>
                userRole.UserId == storedUser.Id
                && userRole.RoleId == bootstrapRole.Id)),
            Times.Once);
    }

    [Fact]
    public async Task ShouldRegisterUserWithoutBootstrapTenantRoleAssignmentWhenUserRolesAlreadyExist()
    {
        RegisterUser input = new()
        {
            DisplayName = "Setup Admin",
            Email = "setup.admin@example.com",
            Password = "Password1!",
            PhoneNumber = "123",
            TenantId = "tenant-1"
        };

        SSOUser storedUser = new()
        {
            Id = "setup.admin",
            DisplayName = input.DisplayName,
            Email = input.Email
        };

        ssoUserProcessingServiceMock
            .Setup(x => x.RegisterSSOUserAsync(It.IsAny<SSOUser>()))
            .ReturnsAsync(storedUser);
        userRoleProcessingServiceMock
            .Setup(x => x.GetAllSSOUserRoles())
            .Returns(new[] { new SSOUserRole { UserId = "existing-admin" } }.AsQueryable());
        tokenProcessingServiceMock
            .Setup(x => x.GenerateConfirmationToken(storedUser.Id))
            .ReturnsAsync(new Token { Id = "token-1" });

        await ssoUserRegistrationOrchestrationService.Register(input);

        roleProcessingServiceMock.Verify(x => x.GetAllSSORoles(), Times.Never);
        userRoleOrchestrationServiceMock.Verify(
            x => x.AddSSOUserRoleAsync(It.IsAny<SSOUserRole>()),
            Times.Never);
    }
}
