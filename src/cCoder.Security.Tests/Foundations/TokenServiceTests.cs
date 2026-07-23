// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations;
using cCoder.Security.Services.Foundations.Interfaces;
using FizzWare.NBuilder;
using Moq;

namespace cCoder.Security.Tests.Foundations;

public partial class TokenServiceTests
{
    private readonly Mock<ITokenBroker> tokenBrokerMock;
    private readonly ITokenService tokenService;

    public TokenServiceTests()
    {
        tokenBrokerMock = new Mock<ITokenBroker>();
        tokenService = new TokenService(tokenBrokerMock.Object, null);
    }

    private static string RandomString() =>
        new RandomGenerator().NextString(minLength: 5, maxLength: 12);

    private static IQueryable<Token> RandomTokens() =>
        Enumerable.Range(start: 0, count: new Random().Next(100))
            .Select(selector: i => RandomToken(id: RandomString()))
            .AsQueryable();

    private static Token RandomToken(string id) =>
        Builder<Token>
            .CreateNew()
            .With(func: i => i.Id = id)
            .Build();
}