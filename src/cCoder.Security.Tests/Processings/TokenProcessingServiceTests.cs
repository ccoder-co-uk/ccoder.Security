// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Caching;
using cCoder.Security.Brokers.Encryption.Interfaces;
using cCoder.Security.Services.Foundations.Interfaces;
using Moq;

namespace cCoder.Security.Tests.Processings;

public sealed partial class TokenProcessingServiceTests
{
    private readonly Mock<ITokenService> tokenServiceMock = new();
    private readonly Mock<ITokenGenerationBroker> tokenGenerationBrokerMock = new();
    private readonly Mock<IPasswordHashingBroker> passwordHashingBrokerMock = new();
    private readonly Mock<ITokenValidationCacheBroker> tokenValidationCacheBrokerMock = new();
}