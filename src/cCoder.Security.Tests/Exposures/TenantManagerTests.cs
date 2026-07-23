// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.Models;
using cCoder.Security.Exposures;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Managements.Interfaces;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Exposures;

public class TenantManagerTests
{
    private readonly Mock<ITenantSetupManagementService> tenantSetupManagementServiceMock;
    private readonly ITenantManager tenantManager;

    public TenantManagerTests()
    {
        tenantSetupManagementServiceMock =
            new Mock<ITenantSetupManagementService>(MockBehavior.Strict);

        tenantManager = new TenantManager(
            tenantSetupManagementService:
                tenantSetupManagementServiceMock.Object);
    }

    [Fact]
    public async Task ShouldProcessTenantSetup()
    {
        SetupDetails setupDetails = CreateSetupDetails();

        tenantSetupManagementServiceMock
            .Setup(expression: service => service.SetupDetailsAsync(setupDetails))
            .Returns(value: ValueTask.CompletedTask);

        await tenantManager.SetupAsync(setupDetails: setupDetails);

        tenantSetupManagementServiceMock.Verify(
            expression: service => service.SetupDetailsAsync(
                setupDetails: setupDetails),
            times: Times.Once);
    }

    private static SetupDetails CreateSetupDetails() =>
        new()
        {
            Tenant = new Tenant
            {
                Id = "default",
                Name = "Default"
            },
            User = new SSOUser
            {
                Id = "admin",
                DisplayName = "Admin User",
                Email = "admin@example.com",
                PasswordHash = "TestPass01!"
            }
        };
}