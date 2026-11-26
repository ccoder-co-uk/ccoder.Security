// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;
using cCoder.Security.Services.Aggregations;
using cCoder.Security.Services.Orchestrations;
using cCoder.Security.Services.Aggregations.Interfaces;
using cCoder.Security.Services.Processings;
using cCoder.Security.Services.Processings.Interfaces;
using cCoder.Security.Brokers.Events;
using cCoder.Security.Exposures;
using cCoder.Security.Events;
using cCoder.Security.Models.Exceptions;
using System;
using System.Threading.Tasks;
using Moq;

namespace cCoder.Security.Tests.Aggregations;

public partial class TenantAggregationServiceTests
{
    private readonly Mock<ITenantProcessingService> tenantProcessingServiceMock;
    private readonly Mock<ISSOUserProcessingService> userProcessingServiceMock;
    private readonly Mock<ISSORoleProcessingService> roleProcessingServiceMock;
    private readonly Mock<ISSOUserRoleProcessingService> userRoleProcessingServiceMock;
    private readonly Mock<IAuthorizationProcessingService> authorizationProcessingServiceMock;
    private readonly Mock<ITenantAnalysisProcessingService> tenantAnalysisProcessingServiceMock;
    private readonly Mock<ITenantDeletedEventBroker> tenantDeletedEventBrokerMock;
    private readonly Mock<ITenantDeletingHandler> tenantDeletingHandlerMock;
    private readonly ITenantAggregationService tenantAggregationService;

    public TenantAggregationServiceTests()
    {
        tenantProcessingServiceMock = new Mock<ITenantProcessingService>(MockBehavior.Strict);
        userProcessingServiceMock = new Mock<ISSOUserProcessingService>(MockBehavior.Strict);

        roleProcessingServiceMock =
            new Mock<ISSORoleProcessingService>(MockBehavior.Strict);

        userRoleProcessingServiceMock =
            new Mock<ISSOUserRoleProcessingService>(MockBehavior.Strict);

        authorizationProcessingServiceMock =
            new Mock<IAuthorizationProcessingService>(MockBehavior.Strict);

        tenantAnalysisProcessingServiceMock =
            new Mock<ITenantAnalysisProcessingService>(MockBehavior.Strict);

        tenantDeletedEventBrokerMock =
            new Mock<ITenantDeletedEventBroker>(MockBehavior.Strict);
        tenantDeletingHandlerMock = new Mock<ITenantDeletingHandler>(MockBehavior.Strict);

        tenantDeletedEventBrokerMock
            .Setup(expression: broker => broker.RaiseTenantDeletedEventAsync(
                message: It.IsAny<cCoder.Eventing.Models.EventMessage<cCoder.Security.Models.Events.SecurityAccountEvent>>()))
            .Returns(value: ValueTask.CompletedTask);

        tenantAggregationService = new TenantOrchestrationService(
            tenantProcessingService: tenantProcessingServiceMock.Object,
            userProcessingService: userProcessingServiceMock.Object,
            roleProcessingService: roleProcessingServiceMock.Object,
            userRoleProcessingService: userRoleProcessingServiceMock.Object,
            authorizationProcessingService:
                authorizationProcessingServiceMock.Object,
            tenantDeletingCleanupHandler: new TenantDeletingCleanupHandler(
                roleProcessingService: roleProcessingServiceMock.Object,
                userRoleProcessingService: userRoleProcessingServiceMock.Object,
                tenantAnalysisProcessingService: tenantAnalysisProcessingServiceMock.Object),
            tenantDeletedEventBroker:
                tenantDeletedEventBrokerMock.Object,
            tenantDeletingHandlers: [tenantDeletingHandlerMock.Object]);
    }

    [Xunit.Fact]
    public async Task DeleteTenantAsync_WhenPreDeleteHandlerRejects_DoesNotDelete()
    {
        Tenant tenant = new() { Id = "tenant-with-bucket-tree" };
        MockSequence sequence = new();
        authorizationProcessingServiceMock.InSequence(sequence)
            .Setup(service => service.EnsureUserIsPortalAdminWithPrivilege("tenant_delete"));
        tenantDeletingHandlerMock.InSequence(sequence)
            .Setup(handler => handler.HandleAsync(tenant.Id))
            .ThrowsAsync(new TenantDeletionRejectedException(reason: TenantDeletionRejection.LinkedBucketTree));

        await Xunit.Assert.ThrowsAsync<TenantDeletionRejectedException>(
            async () => await tenantAggregationService.DeleteTenantAsync(tenant));

        tenantProcessingServiceMock.Verify(
            service => service.DeleteTenantAsync(It.IsAny<Tenant>()), Times.Never);
        tenantDeletedEventBrokerMock.Verify(
            broker => broker.RaiseTenantDeletedEventAsync(
                It.IsAny<cCoder.Eventing.Models.EventMessage<cCoder.Security.Models.Events.SecurityAccountEvent>>()),
            Times.Never);
    }
}