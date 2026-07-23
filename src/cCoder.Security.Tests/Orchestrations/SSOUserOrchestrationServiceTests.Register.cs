// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
            .ReturnsAsync(value: storedUser);
        userRoleProcessingServiceMock
            .Setup(x => x.GetAllSSOUserRoles())
            .Returns(value: Array.Empty<SSOUserRole>().AsQueryable());
        roleProcessingServiceMock
            .Setup(x => x.GetAllSSORoles(true))
            .Returns(value: new[] { bootstrapRole }.AsQueryable());
        userRoleOrchestrationServiceMock
            .Setup(x => x.AddSSOUserRoleAsync(It.IsAny<SSOUserRole>()))
            .ReturnsAsync(valueFunction: (SSOUserRole userRole) => userRole);
        tokenProcessingServiceMock
            .Setup(x => x.GenerateConfirmationToken(storedUser.Id))
            .ReturnsAsync(value: new Token { Id = "token-1" });
        SetupRegistrationCreatedEvent(user: storedUser, registerForm: input, token: "token-1");

        (SSOUser actualUser, string token) =
            await ssoUserOrchestrationService.Register(registerForm: input);

        actualUser.Should().BeSameAs(expected: storedUser);
        token.Should().Be(expected: "token-1");
        userRoleOrchestrationServiceMock.Verify(
expression: x => x.AddSSOUserRoleAsync(It.Is<SSOUserRole>(userRole =>
                userRole.UserId == storedUser.Id
                && userRole.RoleId == bootstrapRole.Id)),
times: Times.Once);
        accountEventServiceMock.Verify(
expression: service => service.RaiseRegistrationCreatedEventAsync(storedUser, input, "token-1"),
times: Times.Once);
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
            .ReturnsAsync(value: storedUser);
        userRoleProcessingServiceMock
            .Setup(x => x.GetAllSSOUserRoles())
            .Returns(value: new[] { new SSOUserRole { UserId = "existing-admin" } }.AsQueryable());
        tokenProcessingServiceMock
            .Setup(x => x.GenerateConfirmationToken(storedUser.Id))
            .ReturnsAsync(value: new Token { Id = "token-1" });
        SetupRegistrationCreatedEvent(user: storedUser, registerForm: input, token: "token-1");

        await ssoUserOrchestrationService.Register(registerForm: input);

        roleProcessingServiceMock.Verify(expression: x => x.GetAllSSORoles(), times: Times.Never);
        userRoleOrchestrationServiceMock.Verify(
expression: x => x.AddSSOUserRoleAsync(It.IsAny<SSOUserRole>()),
times: Times.Never);
        accountEventServiceMock.Verify(
expression: service => service.RaiseRegistrationCreatedEventAsync(storedUser, input, "token-1"),
times: Times.Once);
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
            .ThrowsAsync(exception: new ValidationException("Email exists"));
        ssoUserProcessingServiceMock
            .Setup(service => service.GetAllSSOUsers(true))
            .Returns(value: new[] { existingUser }.AsQueryable());

        (SSOUser actualUser, string token) =
            await ssoUserOrchestrationService.Register(registerForm: input);

        actualUser.Should().BeSameAs(expected: existingUser);
        actualUser.PasswordHash.Should().BeNull();
        token.Should().BeNull();
        tokenProcessingServiceMock.Verify(
expression: service => service.GenerateConfirmationToken(It.IsAny<string>()),
times: Times.Never);
        accountEventServiceMock.Verify(
expression: service => service.RaiseRegistrationCreatedEventAsync(
                It.IsAny<SSOUser>(),
                It.IsAny<RegisterUser>(),
                It.IsAny<string>()),
times: Times.Never);
    }
}