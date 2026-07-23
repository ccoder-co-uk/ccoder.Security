// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Aggregations;
using cCoder.Security.Services.Aggregations.Interfaces;
using cCoder.Security.Services.Processings;
using cCoder.Security.Services.Processings.Interfaces;
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

        tenantAggregationService = new TenantAggregationService(
            tenantProcessingService: tenantProcessingServiceMock.Object,
            userProcessingService: userProcessingServiceMock.Object,
            roleProcessingService: roleProcessingServiceMock.Object,
            userRoleProcessingService: userRoleProcessingServiceMock.Object,
            authorizationProcessingService:
                authorizationProcessingServiceMock.Object,
            tenantAnalysisProcessingService:
                tenantAnalysisProcessingServiceMock.Object);
    }
}