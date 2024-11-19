using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.Entities;
using Microsoft.EntityFrameworkCore;

namespace cCoder.Security.Data.Brokers.Storage;

public class TokenBroker : ITokenBroker
{
    private readonly ISecurityDbContextFactory contextFactory;

    public TokenBroker(ISecurityDbContextFactory contextFactory)
    {
        this.contextFactory = contextFactory;
    }

    public async ValueTask<Token> AddTokenAsync(Token token)
    {
        using EF.SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Token> entityEntry = context.Tokens.Add(token);
        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask<Token> UpdateTokenAsync(Token token)
    {
        using EF.SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Token> entityEntry = context.Tokens.Update(token);
        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask DeleteTokenAsync(Token token)
    {
        using EF.SecurityDbContext context = contextFactory.CreateDbContext();

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Token> entityEntry = context.Tokens.Remove(token);
        await context.SaveChangesAsync();
    }

    public IQueryable<Token> GetAllTokens(bool ignoreFilters = false)
    {
        EF.SecurityDbContext context = contextFactory.CreateDbContext(ignoreFilters);

        return ignoreFilters
            ? context.Tokens.IgnoreQueryFilters()
            : context.Tokens;
    }
}