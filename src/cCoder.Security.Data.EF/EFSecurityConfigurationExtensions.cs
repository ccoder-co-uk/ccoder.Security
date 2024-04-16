using Microsoft.Extensions.DependencyInjection;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects;

namespace cCoder.Security.Data.EF
{
    public static class EFSecurityConfigurationExtensions
    {
        public static void AddEntityFramework(this SecurityConfiguration config, IServiceCollection services)
        {
            services.AddDbContextFactory<SecurityDbContext>();
            services.AddTransient<ISecurityDbContextFactory, SecurityDbContextFactory>();
        }
    }
}