using Moq;
using cCoder.Security.Services.Orchestration;
using cCoder.Security.Services.Orchestration.Interfaces;
using cCoder.Security.Services.Processing.Interfaces;
using System.Text;
using System;
using Tynamix.ObjectFiller;
using Xunit;

namespace cCoder.Security.Services.Tests.Orchestration
{
    public partial class AuthenticationOrchestrationServiceTests
    {
        private readonly Mock<ISessionProcessingService> sessionProcessingServiceMock;
        private readonly Mock<ITokenProcessingService> tokenProcessingServiceMock;
        private readonly Mock<ISSOUserProcessingService> ssoUserProcessingServiceMock;
        private readonly IAuthenticationOrchestrationService authenticationOrchestrationService;

        public AuthenticationOrchestrationServiceTests()
        {
            sessionProcessingServiceMock = new Mock<ISessionProcessingService>();
            tokenProcessingServiceMock = new Mock<ITokenProcessingService>();
            ssoUserProcessingServiceMock = new Mock<ISSOUserProcessingService>();
            authenticationOrchestrationService = new AuthenticationOrchestrationService(
                ssoUserProcessingServiceMock.Object,
                tokenProcessingServiceMock.Object,
                sessionProcessingServiceMock.Object);
        }

        static string RandomString()
            => new MnemonicString().GetValue();
    }
}
