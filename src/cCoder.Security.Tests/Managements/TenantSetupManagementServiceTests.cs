// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Services.Aggregations.Interfaces;
using cCoder.Security.Services.Managements;
using cCoder.Security.Services.Managements.Interfaces;
using Moq;

namespace cCoder.Security.Tests.Managements;

public partial class TenantSetupManagementServiceTests
{
    private readonly Mock<ITenantAggregationService> tenantAggregationServiceMock;
    private readonly Mock<ISSOUserAggregationService> ssoUserAggregationServiceMock;
    private readonly ITenantSetupManagementService tenantSetupManagementService;

    public TenantSetupManagementServiceTests()
    {
        tenantAggregationServiceMock =
            new Mock<ITenantAggregationService>(MockBehavior.Strict);
        ssoUserAggregationServiceMock =
            new Mock<ISSOUserAggregationService>(MockBehavior.Strict);

        tenantSetupManagementService = new TenantSetupManagementService(
            tenantAggregationService: tenantAggregationServiceMock.Object,
            ssoUserAggregationService: ssoUserAggregationServiceMock.Object);
    }
}