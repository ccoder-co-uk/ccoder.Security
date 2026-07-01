using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Moq;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace cCoder.Security.Tests.Orchestrations;

public partial class SSOUserOrchestrationServiceTests
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
        SetupRegistrationCreatedEvent(storedUser, input, "token-1");

        (SSOUser actualUser, string token) =
            await ssoUserOrchestrationService.Register(input);

        actualUser.Should().BeSameAs(storedUser);
        token.Should().Be("token-1");
        userRoleOrchestrationServiceMock.Verify(
            x => x.AddSSOUserRoleAsync(It.Is<SSOUserRole>(userRole =>
                userRole.UserId == storedUser.Id
                && userRole.RoleId == bootstrapRole.Id)),
            Times.Once);
        accountEventServiceMock.Verify(
            service => service.RaiseRegistrationCreatedEventAsync(storedUser, input, "token-1"),
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
        SetupRegistrationCreatedEvent(storedUser, input, "token-1");

        await ssoUserOrchestrationService.Register(input);

        roleProcessingServiceMock.Verify(x => x.GetAllSSORoles(), Times.Never);
        userRoleOrchestrationServiceMock.Verify(
            x => x.AddSSOUserRoleAsync(It.IsAny<SSOUserRole>()),
            Times.Never);
        accountEventServiceMock.Verify(
            service => service.RaiseRegistrationCreatedEventAsync(storedUser, input, "token-1"),
            Times.Once);
    }

    [Fact]
    public async Task ShouldReturnExistingUserWithoutTokenWhenRegisterEmailAlreadyExists()
    {
        RegisterUser input = new()
        {
            DisplayName = "Existing User",
            Email = "existing.user@example.com",
            Password = "Password1!",
            PhoneNumber = "123"
        };

        SSOUser existingUser = new()
        {
            Id = "existing.user",
            DisplayName = input.DisplayName,
            Email = input.Email,
            PasswordHash = "secret"
        };

        ssoUserProcessingServiceMock
            .Setup(service => service.RegisterSSOUserAsync(It.IsAny<SSOUser>()))
            .ThrowsAsync(new ValidationException("Email exists"));
        ssoUserProcessingServiceMock
            .Setup(service => service.GetAllSSOUsers(true))
            .Returns(new[] { existingUser }.AsQueryable());

        (SSOUser actualUser, string token) =
            await ssoUserOrchestrationService.Register(input);

        actualUser.Should().BeSameAs(existingUser);
        actualUser.PasswordHash.Should().BeNull();
        token.Should().BeNull();
        tokenProcessingServiceMock.Verify(
            service => service.GenerateConfirmationToken(It.IsAny<string>()),
            Times.Never);
        accountEventServiceMock.Verify(
            service => service.RaiseRegistrationCreatedEventAsync(
                It.IsAny<SSOUser>(),
                It.IsAny<RegisterUser>(),
                It.IsAny<string>()),
            Times.Never);
    }
}

