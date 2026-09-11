// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Exposures;
using cCoder.Security.Exposures.Controllers;
using cCoder.Security.Models.Entities;
using cCoder.Security.Services.Aggregations.Interfaces;
using cCoder.Security.Services.Orchestrations.Interfaces;
using cCoder.Security.Services.Processings;
using cCoder.Security.Services.Processings.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace cCoder.Security.Tests;

public sealed partial class ControllerHttpComplianceTests
{
    [Fact]
    public void SSOPrivilegeGetByKey_WhenRecordExists_ReturnsRecord()
    {
        // Given
        SSOPrivilege expectedSSOPrivilege = new() { Id = "privilege" };
        Mock<ISSOPrivilegeManager> manager = new();

        manager.Setup(expression: instance => instance.GetAllSSOPrivileges())
            .Returns(value: new[] { expectedSSOPrivilege }.AsQueryable());

        SSOPrivilegeController controller = new(
            privilegeProcessingService: manager.Object);

        // When
        IActionResult result = controller.Get(key: expectedSSOPrivilege.Id);

        // Then
        result
            .Should()
            .BeOfType<OkObjectResult>()
            .Which.Value
            .Should()
            .BeSameAs(expected: expectedSSOPrivilege);
    }

    [Fact]
    public void SSORoleGetByKey_WhenRecordExists_ReturnsRecord()
    {
        // Given
        SSORole expectedSSORole = new() { Id = Guid.NewGuid() };
        Mock<ISSORoleManager> manager = new();

        manager.Setup(expression: instance => instance.GetAllSSORoles())
            .Returns(value: new[] { expectedSSORole }.AsQueryable());

        SSORoleController controller = new(
            roleOrchestrationService: manager.Object);

        // When
        IActionResult result = controller.Get(key: expectedSSORole.Id);

        // Then
        result
            .Should()
            .BeOfType<OkObjectResult>()
            .Which.Value
            .Should()
            .BeSameAs(expected: expectedSSORole);
    }

    [Fact]
    public void SSOUserGetByKey_WhenRecordExists_ReturnsRecord()
    {
        // Given
        SSOUser expectedSSOUser = new() { Id = "user" };
        Mock<ISSOUserManager> manager = new();

        manager.Setup(expression: instance => instance.GetAllSSOUsers())
            .Returns(value: new[] { expectedSSOUser }.AsQueryable());

        SSOUserController controller = new(
            ssoUserAggregationService: manager.Object);

        // When
        IActionResult result = controller.Get(key: expectedSSOUser.Id);

        // Then
        result
            .Should()
            .BeOfType<OkObjectResult>()
            .Which.Value
            .Should()
            .BeSameAs(expected: expectedSSOUser);
    }

    [Fact]
    public void TenantAnalysisGetByKey_WhenRecordExists_ReturnsRecord()
    {
        // Given
        TenantAnalysis expectedTenantAnalysis = new() { Id = Guid.NewGuid() };
        Mock<ITenantAnalysisManager> manager = new();

        manager.Setup(expression: instance => instance.GetAllTenantAnalysis())
            .Returns(value: new[] { expectedTenantAnalysis }.AsQueryable());

        TenantAnalysisController controller = new(
            tenantAnalysisProcessingService: manager.Object);

        // When
        IActionResult result = controller.Get(key: expectedTenantAnalysis.Id);

        // Then
        result
            .Should()
            .BeOfType<OkObjectResult>()
            .Which.Value
            .Should()
            .BeSameAs(expected: expectedTenantAnalysis);
    }

    [Fact]
    public void TenantGetByKey_WhenRecordExists_ReturnsRecord()
    {
        // Given
        Tenant expectedTenant = new() { Id = "tenant" };
        Mock<ITenantAdministrationManager> manager = new();

        manager.Setup(expression: instance => instance.GetAllTenants())
            .Returns(value: new[] { expectedTenant }.AsQueryable());

        TenantController controller = new(
            tenantAggregationService: manager.Object);

        // When
        IActionResult result = controller.Get(key: expectedTenant.Id);

        // Then
        result
            .Should()
            .BeOfType<OkObjectResult>()
            .Which.Value
            .Should()
            .BeSameAs(expected: expectedTenant);
    }

    [Fact]
    public void UserEventGetByKey_WhenRecordExists_ReturnsRecord()
    {
        // Given
        UserEvent expectedUserEvent = new() { Id = Guid.NewGuid() };
        Mock<IUserEventManager> manager = new();

        manager.Setup(expression: instance => instance.GetAllUserEvents())
            .Returns(value: new[] { expectedUserEvent }.AsQueryable());

        UserEventController controller = new(
            userEventProcessingService: manager.Object);

        // When
        IActionResult result = controller.Get(key: expectedUserEvent.Id);

        // Then
        result
            .Should()
            .BeOfType<OkObjectResult>()
            .Which.Value
            .Should()
            .BeSameAs(expected: expectedUserEvent);
    }
}