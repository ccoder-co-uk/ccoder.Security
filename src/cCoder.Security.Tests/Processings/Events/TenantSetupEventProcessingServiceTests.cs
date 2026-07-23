// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Security.Data.Models;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestrations.Interfaces;
using cCoder.Security.Services.Processings.Events;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Processings.Events;

public class TenantSetupEventProcessingServiceTests
{
    private readonly Mock<ITenantSetupOrchestrationService> tenantSetupOrchestrationServiceMock;
    private readonly ITenantSetupEventProcessingService tenantSetupEventProcessingService;

    public TenantSetupEventProcessingServiceTests()
    {
        tenantSetupOrchestrationServiceMock = new Mock<ITenantSetupOrchestrationService>(MockBehavior.Strict);
        tenantSetupEventProcessingService = new TenantSetupEventProcessingService(tenantSetupOrchestrationServiceMock.Object);
    }

    [Fact]
    public async Task ShouldValidateAndNormalizeSetupDetailsBeforeDelegating()
    {
        SetupDetails setupDetails = new()
        {
            Tenant = new Tenant
            {
                Id = "default",
                Name = "Default"
            },
            User = new SSOUser
            {
                DisplayName = "Admin User",
                Email = "admin@example.com",
                PasswordHash = "TestPass01!"
            }
        };

        tenantSetupOrchestrationServiceMock
            .Setup(expression: service => service.SetupDetailsAsync(It.Is<SetupDetails>(details =>
                details.User.Id == "admin"
                && details.Tenant.CreatedBy == "admin"
                && details.Tenant.LastUpdatedBy == "admin"
                && details.Tenant.Description == "Default tenant"
                && details.Tenant.CreatedOn != default
                && details.Tenant.LastUpdated != default)))
            .Returns(value: ValueTask.CompletedTask);

        await tenantSetupEventProcessingService.SetupDetailsAsync(setupDetails: setupDetails);

        tenantSetupOrchestrationServiceMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldRejectIncompleteSetupDetails()
    {
        SetupDetails missingTenantId = new()
        {
            Tenant = new Tenant { Name = "Default" },
            User = new SSOUser
            {
                DisplayName = "Admin User",
                Email = "admin@example.com",
                PasswordHash = "TestPass01!"
            }
        };

        Func<Task> act = async () => await tenantSetupEventProcessingService.SetupDetailsAsync(setupDetails: missingTenantId);

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage(expectedWildcardPattern: "Tenant ID is required.");
    }
}