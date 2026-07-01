using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Exposures;

public interface ITokenManager
{
    ValueTask<Token> IssueTokenAsync(string userId, TokenUse tokenUse);
}
