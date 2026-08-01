// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Configuration;
using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Brokers.Encryption.Interfaces;
using cCoder.Security.Models.Entities;
using cCoder.Security.Services.Foundations;
using cCoder.Security.Services.Foundations.Interfaces;
using FizzWare.NBuilder;
using Moq;

namespace cCoder.Security.Tests.Foundations;

public partial class TokenServiceTests
{
    private readonly Mock<ITokenBroker> tokenBrokerMock;
    private readonly Mock<ISecurityConfigurationBroker> configurationBrokerMock;
    private readonly Mock<ITokenGenerationBroker> tokenGenerationBrokerMock;
    private readonly Mock<IPasswordHashingBroker> passwordHashingBrokerMock;
    private readonly ITokenService tokenService;

    public TokenServiceTests()
    {
        tokenBrokerMock = new Mock<ITokenBroker>();
        configurationBrokerMock = new Mock<ISecurityConfigurationBroker>();
        tokenGenerationBrokerMock = new Mock<ITokenGenerationBroker>();
        passwordHashingBrokerMock = new Mock<IPasswordHashingBroker>();

        tokenService = new TokenService(
            tokenBroker: tokenBrokerMock.Object,
            configurationBroker: configurationBrokerMock.Object,
            tokenGenerationBroker: tokenGenerationBrokerMock.Object,
            passwordHashingBroker: passwordHashingBrokerMock.Object);
    }

    private static string RandomString() =>
        new RandomGenerator().NextString(minLength: 5, maxLength: 12);

    private static IQueryable<Token> RandomTokens() =>
        Enumerable.Range(start: 0, count: new Random().Next(maxValue: 100))
            .Select(selector: i => RandomToken(id: RandomString()))
            .AsQueryable();

    private static Token RandomToken(string id) =>
        Builder<Token>
            .CreateNew()
            .With(func: i => i.Id = id)
            .Build();
}