using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace cCoder.Security.Brokers.Storage;
internal class TokenBroker(ISecurityDbContextFactory contextFactory) 
    : ITokenBroker
{
    public async ValueTask<Token> AddTokenAsync(Token token)
    {
        using SecurityDbContext context = 
            contextFactory.CreateDbContext();

        EntityEntry<Token> entityEntry =
            context.Tokens.Add(token);

        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask<Token> UpdateTokenAsync(Token token)
    {
        using SecurityDbContext context = 
            contextFactory.CreateDbContext();

        EntityEntry<Token> entityEntry = 
            context.Tokens.Update(token);

        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask DeleteTokenAsync(Token token)
    {
        using SecurityDbContext context = 
            contextFactory.CreateDbContext();

        EntityEntry<Token> entityEntry = 
            context.Tokens.Remove(token);

        await context.SaveChangesAsync();
    }

    public async ValueTask<int> DeleteExpiredAsync(
        DateTimeOffset expiresBefore,
        CancellationToken cancellationToken = default)
    {
        using SecurityDbContext context = contextFactory.CreateDbContext(ignoreAuthInfo: true);

        int deletedCount = await context.Tokens
            .IgnoreQueryFilters()
            .Where(token => token.Expires < expiresBefore)
            .ExecuteDeleteAsync(cancellationToken);

        return deletedCount;
    }

    public IQueryable<Token> GetAllTokens(bool ignoreFilters = false)
    {
        SecurityDbContext context = 
            contextFactory.CreateDbContext(ignoreFilters);

        return ignoreFilters
            ? context.Tokens.IgnoreQueryFilters()
            : context.Tokens;
    }
}
