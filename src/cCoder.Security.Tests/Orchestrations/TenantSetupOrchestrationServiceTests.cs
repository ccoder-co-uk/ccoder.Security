using cCoder.Security.Data.Models;
using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestrations;
using cCoder.Security.Services.Orchestrations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;
using Moq;

namespace cCoder.Security.Tests.Orchestrations;

public partial class TenantSetupOrchestrationServiceTests
{
    private readonly Mock<ITenantOrchestrationService> tenantOrchestrationServiceMock;
    private readonly Mock<ISSOUserOrchestrationService> ssoUserOrchestrationServiceMock;
    private readonly ITenantSetupOrchestrationService tenantSetupOrchestrationService;

    public TenantSetupOrchestrationServiceTests()
    {
        tenantOrchestrationServiceMock = new Mock<ITenantOrchestrationService>(MockBehavior.Strict);
        ssoUserOrchestrationServiceMock = new Mock<ISSOUserOrchestrationService>(MockBehavior.Strict);

        tenantSetupOrchestrationService = new TenantSetupOrchestrationService(
            tenantOrchestrationServiceMock.Object,
            ssoUserOrchestrationServiceMock.Object);
    }
}

