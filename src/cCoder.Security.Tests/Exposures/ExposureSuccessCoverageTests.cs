// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Exposures.Controllers;
using cCoder.Security.Exposures;
using cCoder.Security.Models.Entities;
using cCoder.Security.Services.Aggregations.Interfaces;
using cCoder.Security.Services.Orchestrations.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Exposures;

public sealed partial class ExposureSuccessCoverageTests
{
    [Fact]
    public async Task ShouldDeleteExistingEntitiesThroughEveryExposure()
    {
        // Given

        SSORole role = new() { Id = Guid.NewGuid() };
        SSOUser user = new() { Id = "coverage-user" };
        Tenant tenant = new() { Id = "coverage-tenant" };

        SSOUserRole userRole = new()
        {
            UserId = user.Id,
            RoleId = role.Id
        };

        Mock<ISSORoleManager> roles = new();
        Mock<ISSOUserManager> users = new();
        Mock<ITenantAdministrationManager> tenants = new();
        Mock<ISSOUserRoleManager> userRoles = new();

        roles
            .Setup(expression: manager => manager.GetAllSSORoles())
            .Returns(value: new[] { role }.AsQueryable());

        users
            .Setup(expression: manager => manager.GetAllSSOUsers())
            .Returns(value: new[] { user }.AsQueryable());

        tenants
            .Setup(expression: manager => manager.GetAllTenants())
            .Returns(value: new[] { tenant }.AsQueryable());

        userRoles
            .Setup(expression: manager => manager.GetAllSSOUserRoles())
            .Returns(value: new[] { userRole }.AsQueryable());

        SSORoleController roleController = new(
            roleOrchestrationService: roles.Object);

        SSOUserController userController = new(
            ssoUserAggregationService: users.Object);

        TenantController tenantController = new(
            tenantAggregationService: tenants.Object);

        SSOUserRoleController userRoleController = new(
            userRoleOrchestrationService: userRoles.Object);

        // When

        IActionResult roleResult = await roleController.Delete(key: role.Id);

        IActionResult userResult = await userController.Delete(
            key: user.Id,
            reference: null);

        IActionResult tenantResult = await tenantController.Delete(
            key: tenant.Id);

        IActionResult userRoleResult = await userRoleController.Delete(
            userId: user.Id,
            roleId: role.Id);

        // Then

        roleResult
            .Should()
            .BeOfType<NoContentResult>();

        userResult
            .Should()
            .BeOfType<NoContentResult>();

        tenantResult
            .Should()
            .BeOfType<NoContentResult>();

        userRoleResult
            .Should()
            .BeOfType<NoContentResult>();

        roles.Verify(
            expression: manager => manager.DeleteSSORoleAsync(item: role),
            times: Times.Once);

        users.Verify(
            expression: manager =>
                manager.DeleteSSOUserAsync(deletedSSOUser: user),
            times: Times.Once);

        tenants.Verify(
            expression: manager => manager.DeleteTenantAsync(item: tenant),
            times: Times.Once);

        userRoles.Verify(
            expression: manager =>
                manager.DeleteSSOUserRoleAsync(userRole: userRole),
            times: Times.Once);
    }
}