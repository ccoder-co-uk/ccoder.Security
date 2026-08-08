// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Dependencies.EDM;
using cCoder.Security.Dependencies.HostedServices;
using cCoder.Security.Models;
using cCoder.Security.Models.Entities;
using cCoder.Security.Services.Aggregations;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Processings;
using cCoder.Security.Services.Processings.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace cCoder.Security.Tests;

public sealed partial class CoverageGapTests
{
    [Fact]
    public void ShouldBuildNamedAndJoinedODataSets()
    {
        // Given

        CoverageODataModelBuilder builder = new();

        // When

        ODataModel model = builder.Build();

        // Then

        model.EDMModel
            .Should()
            .NotBeNull();
    }

    [Fact]
    public async Task ShouldDeleteTenantAndAllOwnedRelationships()
    {
        // Given

        Tenant tenant = new() { Id = "tenant-coverage" };
        SSORole role = new() { Id = Guid.NewGuid(), TenantId = tenant.Id };
        SSOUserRole userRole = new() { RoleId = role.Id };
        TenantAnalysis analysis = new() { Id = Guid.NewGuid(), TenantId = tenant.Id };

        Mock<ITenantProcessingService> tenants = new(MockBehavior.Strict);
        Mock<ISSOUserProcessingService> users = new(MockBehavior.Strict);
        Mock<ISSORoleProcessingService> roles = new(MockBehavior.Strict);
        Mock<ISSOUserRoleProcessingService> userRoles = new(MockBehavior.Strict);
        Mock<IAuthorizationProcessingService> authorization = new(MockBehavior.Strict);
        Mock<ITenantAnalysisProcessingService> analyses = new(MockBehavior.Strict);

        authorization
            .Setup(expression: service =>
                service.EnsureUserIsPortalAdminWithPrivilege(
                    privilege: "tenant_delete"));

        roles
            .Setup(expression: service => service.GetAllSSORoles())
            .Returns(value: new[] { role }.AsQueryable());

        userRoles
            .Setup(expression: service => service.GetAllSSOUserRoles())
            .Returns(value: new[] { userRole }.AsQueryable());

        analyses
            .Setup(expression: service => service.GetAllTenantAnalysis())
            .Returns(value: new[] { analysis }.AsQueryable());

        analyses
            .Setup(expression: service =>
                service.DeleteTenantAnalysisAsync(item: analysis))
            .Returns(value: ValueTask.CompletedTask);

        userRoles
            .Setup(expression: service =>
                service.DeleteSSOUserRoleAsync(item: userRole))
            .Returns(value: ValueTask.CompletedTask);

        roles
            .Setup(expression: service => service.DeleteSSORoleAsync(item: role))
            .Returns(value: ValueTask.CompletedTask);

        tenants
            .Setup(expression: service => service.DeleteTenantAsync(item: tenant))
            .Returns(value: ValueTask.CompletedTask);

        TenantAggregationService service = new(
            tenantProcessingService: tenants.Object,
            userProcessingService: users.Object,
            roleProcessingService: roles.Object,
            userRoleProcessingService: userRoles.Object,
            authorizationProcessingService: authorization.Object,
            tenantAnalysisProcessingService: analyses.Object);

        // When

        await service.DeleteTenantAsync(deletedTenant: tenant);

        // Then

        tenants.Verify(
            expression: dependency => dependency.DeleteTenantAsync(item: tenant),
            times: Times.Once);

        roles.Verify(
            expression: dependency => dependency.DeleteSSORoleAsync(item: role),
            times: Times.Once);

        userRoles.Verify(
            expression: dependency =>
                dependency.DeleteSSOUserRoleAsync(item: userRole),
            times: Times.Once);

        analyses.Verify(
            expression: dependency =>
                dependency.DeleteTenantAnalysisAsync(item: analysis),
            times: Times.Once);
    }

    [Fact]
    public async Task ShouldRunTokenCleanupOnceBeforeCancellation()
    {
        // Given

        Mock<ITokenService> tokenService = new(MockBehavior.Strict);

        TaskCompletionSource cleanupCompleted = new(
            creationOptions: TaskCreationOptions.RunContinuationsAsynchronously);

        tokenService
            .Setup(expression: service =>
                service.DeleteExpiredAsync(
                    cancellationToken: It.IsAny<CancellationToken>()))
            .Callback(callback: () => cleanupCompleted.SetResult())
            .Returns(value: new ValueTask<int>(result: 1));

        ServiceProvider provider = new ServiceCollection()
            .AddSingleton(implementationInstance: tokenService.Object)
            .BuildServiceProvider();

        SecurityConfiguration configuration = new()
        {
            IsMigrating = false
        };

        TokenCleaner cleaner = new(
            serviceScopeFactory:
                provider.GetRequiredService<IServiceScopeFactory>(),
            securityConfiguration: configuration);

        using CancellationTokenSource cancellation = new();

        // When

        await cleaner.StartAsync(cancellationToken: cancellation.Token);
        await cleanupCompleted.Task;
        cancellation.Cancel();
        await cleaner.StopAsync(cancellationToken: CancellationToken.None);

        // Then

        tokenService.Verify(
            expression: service =>
                service.DeleteExpiredAsync(
                    cancellationToken: It.IsAny<CancellationToken>()),
            times: Times.Once);
    }

    private sealed class CoverageODataModelBuilder : ODataModelBuilder
    {
        public override ODataModel Build()
        {
            AddSet<Tenant, string>(
                enableBatchingToo: true,
                setName: "CoverageTenants");

            AddSet<Tenant, string>();

            AddJoinSet<Tenant, string>(
                key: tenant => tenant.Id);

            return new ODataModel
            {
                EDMModel = Builder.GetEdmModel()
            };
        }
    }
}