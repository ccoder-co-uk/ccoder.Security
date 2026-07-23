// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.Models;
using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Orchestrations;

public partial class TenantSetupOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCreateTenantThenRegisterBootstrapUserForSetup()
    {
        SetupDetails setupDetails = new()
        {
            Tenant = new Tenant
            {
                Id = "default",
                Name = "Default",
                CreatedBy = "admin",
                LastUpdatedBy = "admin",
                Description = "Default tenant",
                CreatedOn = DateTimeOffset.UtcNow,
                LastUpdated = DateTimeOffset.UtcNow
            },
            User = new SSOUser
            {
                Id = "admin",
                DisplayName = "Admin User",
                Email = "admin@example.com",
                PasswordHash = "TestPass01!"
            }
        };

        tenantOrchestrationServiceMock
            .Setup(service => service.AddTenantAsync(setupDetails.Tenant))
            .ReturnsAsync(valueFunction: (Tenant tenant) => tenant);

        ssoUserOrchestrationServiceMock
            .Setup(service => service.Register(It.Is<RegisterUser>(user =>
                user.Email == "admin@example.com"
                && user.DisplayName == "Admin User"
                && user.Password == "TestPass01!"
                && user.TenantId == "default")))
            .ReturnsAsync(value: (new SSOUser { Id = "admin", Email = "admin@example.com" }, "token"));

        ssoUserOrchestrationServiceMock
            .Setup(service => service.ConfirmRegistration("token"))
            .Returns(value: ValueTask.CompletedTask);

        await tenantSetupOrchestrationService.SetupAsync(setupDetails: setupDetails);

        setupDetails.User.Id.Should().Be(expected: "admin");
        tenantOrchestrationServiceMock.VerifyAll();
        ssoUserOrchestrationServiceMock.VerifyAll();
    }
}