namespace cCoder.Security.Data.EF.Interfaces
{
    public interface ISecurityDbContextFactory
    {
        SecurityDbContext CreateDbContext(bool withAuthInfo = true);
    }
}