using Microsoft.EntityFrameworkCore.Design;

namespace cCoder.Security.Data.EF.Interfaces;

public interface ISecurityDbContextFactory : IDesignTimeDbContextFactory<SecurityDbContext>
{
    SecurityDbContext CreateDbContext();
}