using System.Net.Sockets;
using cCoder.Security.Data.Brokers.Utility.Interfaces;
using cCoder.Security.Services.Orchestration;
using cCoder.Security.Services.Orchestration.Interfaces;
using cCoder.Security.Services.Processing.Interfaces;
using FizzWare.NBuilder;
using Moq;
using Tynamix.ObjectFiller;

namespace cCoder.Security.Services.Tests.Orchestration;

public partial class TenantOrchestrationServiceTests
{
    private readonly Mock<ITenantProcessingService> tenantProcessingServiceMock;
    private readonly Mock<ISSORoleProcessingService> roleProcessingServiceMock;
    private readonly Mock<ISSOUserRoleProcessingService> userRoleProcessingServiceMock;
    private readonly Mock<ISSOAuthorizationBroker> authBrokerMock;
    private readonly ITenantOrchestrationService tenantOrchestrationService;

    public TenantOrchestrationServiceTests()
    {
        tenantProcessingServiceMock = new Mock<ITenantProcessingService>();
        roleProcessingServiceMock = new Mock<ISSORoleProcessingService>();
        userRoleProcessingServiceMock = new Mock<ISSOUserRoleProcessingService>();
        authBrokerMock = new Mock<ISSOAuthorizationBroker>();

        tenantOrchestrationService = new TenantOrchestrationService(
            tenantProcessingServiceMock.Object,
            roleProcessingServiceMock.Object,
            userRoleProcessingServiceMock.Object,
        authBrokerMock.Object
        );
    }
}
