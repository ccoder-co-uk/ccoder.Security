// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
    public async ValueTask<Token> InsertTokenAsync(Token newToken)
    {
        using SecurityDbContext context =
            contextFactory.CreateDbContext();

        EntityEntry<Token> entityEntry =
            context.Tokens.Add(entity: newToken);

        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask<Token> UpdateTokenAsync(Token updatedToken)
    {
        using SecurityDbContext context =
            contextFactory.CreateDbContext();

        EntityEntry<Token> entityEntry =
            context.Tokens.Update(entity: updatedToken);

        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask DeleteTokenAsync(Token deletedToken)
    {
        using SecurityDbContext context =
            contextFactory.CreateDbContext();

        EntityEntry<Token> entityEntry =
            context.Tokens.Remove(entity: deletedToken);

        await context.SaveChangesAsync();
    }

    public async ValueTask<int> DeleteExpiredAsync(
        DateTimeOffset expiresBefore,
        CancellationToken cancellationToken = default)
    {
        using SecurityDbContext context = contextFactory.CreateDbContext(ignoreAuthInfo: true);

        int deletedCount = await context.Tokens
            .IgnoreQueryFilters()
            .Where(predicate: token => token.Expires < expiresBefore)
            .ExecuteDeleteAsync(cancellationToken: cancellationToken);

        return deletedCount;
    }

    public IQueryable<Token> SelectAllTokens() =>
        contextFactory
            .CreateDbContext()
            .Tokens;

    public IQueryable<Token> SelectAllTokensIgnoringFilters() =>
        contextFactory
            .CreateDbContext(ignoreAuthInfo: true)
            .Tokens
            .IgnoreQueryFilters();
}