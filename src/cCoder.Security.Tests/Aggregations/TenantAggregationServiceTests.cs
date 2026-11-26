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
using cCoder.Eventing;
using cCoder.Eventing.Brokers;
using cCoder.Eventing.Models.Exceptions;
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
    private readonly Mock<ITenantDeletedEventBroker> tenantDeletedEventBrokerMock;
    private readonly Mock<ILocalEventHandler<string>> tenantDeletingHandlerMock;
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

        tenantDeletedEventBrokerMock =
            new Mock<ITenantDeletedEventBroker>(MockBehavior.Strict);
        tenantDeletingHandlerMock = new Mock<ILocalEventHandler<string>>(MockBehavior.Strict);
        Mock<ILocalEventBroker> localEvents = new(MockBehavior.Strict);

        localEvents.Setup(expression: broker => broker.GetHandlers<string>(name: "tenant_delete"))
            .Returns(value: [tenantDeletingHandlerMock.Object]);

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
            tenantDeletedEventBroker:
                tenantDeletedEventBrokerMock.Object,
            localEventBroker: localEvents.Object);
    }

    [Xunit.Fact]
    public async Task DeleteTenantAsync_WhenPreDeleteHandlerRejects_DoesNotDelete()
    {
        Tenant tenant = new() { Id = "tenant-with-bucket-tree" };
        MockSequence sequence = new();
        authorizationProcessingServiceMock.InSequence(sequence)
            .Setup(service => service.EnsureUserIsPortalAdminWithPrivilege("tenant_delete"));
        tenantDeletingHandlerMock.InSequence(sequence)
            .Setup(handler => handler.HandleAsync(data: tenant.Id))
            .Returns(value: ValueTask.FromException(exception: new LocalEventRejectedException(code: "linked_bucket_tree")));

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
