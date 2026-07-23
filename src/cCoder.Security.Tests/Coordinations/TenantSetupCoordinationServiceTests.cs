// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Services.Coordinations;
using cCoder.Security.Services.Coordinations.Interfaces;
using cCoder.Security.Services.Orchestrations.Interfaces;
using Moq;

namespace cCoder.Security.Tests.Coordinations;

public partial class TenantSetupCoordinationServiceTests
{
    private readonly Mock<ITenantOrchestrationService> tenantOrchestrationServiceMock;
    private readonly Mock<ISSOUserOrchestrationService> ssoUserOrchestrationServiceMock;
    private readonly ITenantSetupCoordinationService tenantSetupCoordinationService;

    public TenantSetupCoordinationServiceTests()
    {
        tenantOrchestrationServiceMock = new Mock<ITenantOrchestrationService>(MockBehavior.Strict);
        ssoUserOrchestrationServiceMock = new Mock<ISSOUserOrchestrationService>(MockBehavior.Strict);

        tenantSetupCoordinationService = new TenantSetupCoordinationService(
            tenantOrchestrationService: tenantOrchestrationServiceMock.Object,
            ssoUserOrchestrationService: ssoUserOrchestrationServiceMock.Object);
    }
}