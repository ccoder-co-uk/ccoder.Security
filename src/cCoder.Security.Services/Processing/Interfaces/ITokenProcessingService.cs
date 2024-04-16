using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Processing.Interfaces;

public interface ITokenProcessingService
{
    IQueryable<Token> GetAllTokens(bool ignoreFilters = false);
    Token GetTokenById(string id);

    Token GetForgottenPasswordToken(string tokenId);
    Token GetConfirmationToken(string tokenId);
    ValueTask<Token> GenerateForgottenPasswordToken(string userId);
    ValueTask<Token> GenerateConfirmationToken(string userId);

    ValueTask<Token> AddTokenForUserIdAsync(string userId);
    ValueTask DeleteTokenAsync(string tokenId);
}