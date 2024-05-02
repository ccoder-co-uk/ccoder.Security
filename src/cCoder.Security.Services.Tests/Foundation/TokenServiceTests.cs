using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation;
using cCoder.Security.Services.Foundation.Interfaces;
using FizzWare.NBuilder;
using Moq;

namespace cCoder.Security.Services.Tests.Foundation;

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
        new RandomGenerator().NextString(5, 12);

    private static IQueryable<Token> RandomTokens() => 
        Enumerable.Range(0, new Random().Next(100))
            .Select(i => RandomToken(RandomString()))
            .AsQueryable();

    private static Token RandomToken(string id) => 
        Builder<Token>
            .CreateNew()
            .With(i => i.Id = id)
            .Build();
}