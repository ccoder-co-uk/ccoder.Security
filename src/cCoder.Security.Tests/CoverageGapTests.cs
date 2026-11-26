// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using System.Threading;
using System;
using System.Threading.Tasks;
using cCoder.Security.Exposures.EDM;
using cCoder.Security.Exposures.HostedServices;
using cCoder.Security.Brokers.Utility;
using cCoder.Security.Brokers.Caching;
using cCoder.Security.Models;
using cCoder.Security.Models.Entities;
using cCoder.Security.Services.Aggregations;
using cCoder.Security.Services.Orchestrations;
using cCoder.Security.Brokers.Encryption.Interfaces;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Processings;
using cCoder.Security.Services.Processings.Interfaces;
using cCoder.Security.Brokers.Events;
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
        Mock<ITenantDeletedEventBroker> tenantDeletedEvents = new(MockBehavior.Strict);
        Mock<cCoder.Security.Exposures.ITenantDeletingHandler> coreGuard = new(MockBehavior.Strict);
        Mock<cCoder.Security.Exposures.ITenantDeletingHandler> b2bGuard = new(MockBehavior.Strict);
        MockSequence deletionOrder = new();

        authorization
            .InSequence(sequence: deletionOrder)
            .Setup(expression: service =>
                service.EnsureUserIsPortalAdminWithPrivilege(
                    privilege: "tenant_delete"));

        coreGuard
            .InSequence(sequence: deletionOrder)
            .Setup(expression: handler => handler.HandleAsync(tenant.Id))
            .Returns(value: ValueTask.CompletedTask);

        b2bGuard
            .InSequence(sequence: deletionOrder)
            .Setup(expression: handler => handler.HandleAsync(tenant.Id))
            .Returns(value: ValueTask.CompletedTask);

        authorization
            .Setup(expression: service => service.GetAuthorizationContext())
            .Returns(value: new AuthorizationContext
            {
                CurrentUser = new SSOUser { Id = "admin" }
            });

        roles
            .Setup(expression: service => service.GetAllSSORoles())
            .Returns(value: new[] { role }.AsQueryable());

        userRoles
            .Setup(expression: service => service.GetAllSSOUserRoles())
            .Returns(value: new[] { userRole }.AsQueryable());

        analyses
            .Setup(expression: service => service.GetAllTenantAnalyses())
            .Returns(value: new[] { analysis }.AsQueryable());

        analyses
            .InSequence(sequence: deletionOrder)
            .Setup(expression: service =>
                service.DeleteTenantAnalysisAsync(item: analysis))
            .Returns(value: ValueTask.CompletedTask);

        userRoles
            .InSequence(sequence: deletionOrder)
            .Setup(expression: service =>
                service.DeleteSSOUserRoleAsync(item: userRole))
            .Returns(value: ValueTask.CompletedTask);

        roles
            .InSequence(sequence: deletionOrder)
            .Setup(expression: service => service.DeleteSSORoleAsync(item: role))
            .Returns(value: ValueTask.CompletedTask);

        tenants
            .InSequence(sequence: deletionOrder)
            .Setup(expression: service => service.DeleteTenantAsync(item: tenant))
            .Returns(value: ValueTask.CompletedTask);

        tenantDeletedEvents
            .InSequence(sequence: deletionOrder)
            .Setup(expression: broker => broker.RaiseTenantDeletedEventAsync(
                message: It.IsAny<cCoder.Eventing.Models.EventMessage<cCoder.Security.Models.Events.SecurityAccountEvent>>()))
            .Returns(value: ValueTask.CompletedTask);

        TenantOrchestrationService service = new(
            tenantProcessingService: tenants.Object,
            userProcessingService: users.Object,
            roleProcessingService: roles.Object,
            userRoleProcessingService: userRoles.Object,
            authorizationProcessingService: authorization.Object,
            tenantDeletingCleanupHandler: new cCoder.Security.Events.TenantDeletingCleanupHandler(
                roleProcessingService: roles.Object,
                userRoleProcessingService: userRoles.Object,
                tenantAnalysisProcessingService: analyses.Object),
            tenantDeletedEventBroker: tenantDeletedEvents.Object,
            tenantDeletingHandlers: [coreGuard.Object, b2bGuard.Object]);

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

        tenantDeletedEvents.Verify(
            expression: broker => broker.RaiseTenantDeletedEventAsync(
                message: It.Is<cCoder.Eventing.Models.EventMessage<cCoder.Security.Models.Events.SecurityAccountEvent>>(
                    match: message => message.Data.Tenant.Id == tenant.Id
                        && message.Data.Kind == cCoder.Security.Models.Events.SecurityAccountEventKind.TenantDeleted)),
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

        SecurityConfiguration configuration = new()
        {
            IsMigrating = false
        };

        TokenProcessingService processingService = new(
            tokenService: tokenService.Object,
            tokenGenerationBroker: Mock.Of<ITokenGenerationBroker>(),
            passwordHashingBroker: Mock.Of<IPasswordHashingBroker>(),
            tokenValidationCacheBroker:
                Mock.Of<ITokenValidationCacheBroker>(),
            securityConfiguration: configuration);

        using ServiceProvider serviceProvider = new ServiceCollection()
            .AddScoped<ITokenProcessingService>(
                implementationFactory: _ => processingService)
            .BuildServiceProvider();

        ServiceScopeBroker serviceScopeBroker = new(
            serviceScopeFactory:
                serviceProvider.GetRequiredService<IServiceScopeFactory>());

        TokenCleaner cleaner = new(
            serviceScopeBroker: serviceScopeBroker);

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