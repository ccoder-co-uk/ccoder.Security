using cCoder.Security.Data.Brokers.DateTime;
using cCoder.Security.Data.Brokers.Requests;
using cCoder.Security.Data.Brokers.Serialization;
using cCoder.Security.Data.Brokers.Storage;
using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects;
using cCoder.Security.Services.Foundation;
using cCoder.Security.Services.Foundation.Interfaces;
using cCoder.Security.Services.Orchestration;
using cCoder.Security.Services.Orchestration.Interfaces;
using cCoder.Security.Services.Processing;
using cCoder.Security.Services.Processing.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Security.Services;

public static class IServiceCollectionExtensions
{
    public static SecurityConfiguration AddSecurityServices(
        this IServiceCollection services,
        Action<IServiceCollection, SecurityConfiguration> configAction)
    {
        var securityConfiguration = new SecurityConfiguration();
        configAction(services, securityConfiguration);

        services.AddScoped<ISecurityDbContextFactory>(sp => 
            new SecurityDbContextFactory(sp.GetService<ISecurityModelBuildProvider>())
            { 
                GetAuthInfo = () => sp.GetService<ISSOAuthInfo>()
            });

        services.AddScoped((IServiceProvider provider) =>
            provider.GetService<ISSOAuthInfoOrchestrationService>().GetSSOAuthInfo());

        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();

        return securityConfiguration;
    }

    static void AddBrokers(this IServiceCollection services)
    {
        services.AddScoped<IHttpRequestBroker, HttpRequestBroker>();
        services.AddScoped<ISessionBroker, SessionBroker>();
        services.AddScoped<ISSOPrivilegeBroker, SSOPrivilegeBroker>();
        services.AddScoped<ISSORoleBroker, SSORoleBroker>();
        services.AddScoped<ISSOUserBroker, SSOUserBroker>();
        services.AddScoped<ISSOUserRoleBroker, SSOUserRoleBroker>();
        services.AddScoped<ITenantBroker, TenantBroker>();
        services.AddScoped<ITenantAnalysisBroker, TenantAnalysisBroker>();
        services.AddScoped<ITokenBroker, TokenBroker>();
        services.AddScoped<IUserEventBroker, UserEventBroker>();

        services.AddScoped<ISerializationBroker, SerializationBroker>();
        services.AddScoped<ISecurityDateTimeOffsetBroker, SecurityDateTimeOffsetBroker>();
    }

    static void AddFoundations(this IServiceCollection services)
    {
        services.AddScoped<ISSOUserService, SSOUserService>();
        services.AddScoped<ISSOPrivilegeService, SSOPrivilegeService>();
        services.AddScoped<ISSOUserRoleService, SSOUserRoleService>();
        services.AddScoped<ISSORoleService, SSORoleService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<ISessionService, SessionService>();
    }

    static void AddProcessings(this IServiceCollection services)
    {
        services.AddScoped<ISSOUserProcessingService, SSOUserProcessingService>();
        services.AddScoped<ISSOPrivilegeProcessingService, SSOPrivilegeProcessingService>();
        services.AddScoped<ISSOUserRoleProcessingService, SSOUserRoleProcessingService>();
        services.AddScoped<ISSORoleProcessingService, SSORoleProcessingService>();
        services.AddScoped<ITokenProcessingService, TokenProcessingService>();
        services.AddScoped<ITenantProcessingService, TenantProcessingService>();
        services.AddScoped<ISessionProcessingService, SessionProcessingService>();
    }

    static void AddOrchestrations(this IServiceCollection services)
    {
        services.AddScoped<ISSOUserOrchestrationService, SSOUserRegistrationOrchestrationService>();
        services.AddScoped<ISSOAuthInfoOrchestrationService, SSOAuthInfoOrchestrationService>();
        services.AddScoped<IAuthenticationOrchestrationService, AuthenticationOrchestrationService>();
    }
}
