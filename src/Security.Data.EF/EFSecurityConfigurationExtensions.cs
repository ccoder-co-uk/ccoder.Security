using Microsoft.Extensions.DependencyInjection;
using Security.Data.EF.Interfaces;
using Security.Objects;

namespace Security.Data.EF
{
    public static class EFSecurityConfigurationExtensions
    {
        public static void AddEntityFramework(this SecurityConfiguration config, IServiceCollection services)
        {
            services.AddDbContextPool<SSODbContext>(opt => { });
            services.AddDbContextFactory<SSODbContext>();
            services.AddTransient<ISSODbContextFactory, SSODbContextFactory>();
        }
    }
}