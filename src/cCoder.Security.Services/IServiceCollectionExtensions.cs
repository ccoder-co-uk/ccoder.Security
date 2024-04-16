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

        services.AddTransient<ISecurityDbContextFactory, SecurityDbContextFactory>();

        services.AddTransient((IServiceProvider provider) =>
            provider.GetService<ISSOAuthInfoOrchestrationService>().GetSSOAuthInfo());

        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();

        return securityConfiguration;
    }

    static void AddBrokers(this IServiceCollection services)
    {
        services.AddTransient<IHttpRequestBroker, HttpRequestBroker>();
        services.AddTransient<ISessionBroker, SessionBroker>();
        services.AddTransient<ISSOPrivilegeBroker, SSOPrivilegeBroker>();
        services.AddTransient<ISSORoleBroker, SSORoleBroker>();
        services.AddTransient<ISSOUserBroker, SSOUserBroker>();
        services.AddTransient<ISSOUserRoleBroker, SSOUserRoleBroker>();
        services.AddTransient<ITenantBroker, TenantBroker>();
        services.AddTransient<ITenantAnalysisBroker, TenantAnalysisBroker>();
        services.AddTransient<ITokenBroker, TokenBroker>();
        services.AddTransient<IUserEventBroker, UserEventBroker>();

        services.AddTransient<ISerializationBroker, SerializationBroker>();
        services.AddTransient<ISecurityDateTimeOffsetBroker, SecurityDateTimeOffsetBroker>();
    }

    static void AddFoundations(this IServiceCollection services)
    {
        services.AddTransient<ISSOUserService, SSOUserService>();
        services.AddTransient<ISSOPrivilegeService, SSOPrivilegeService>();
        services.AddTransient<ISSOUserRoleService, SSOUserRoleService>();
        services.AddTransient<ISSORoleService, SSORoleService>();
        services.AddTransient<ITokenService, TokenService>();
        services.AddTransient<ITenantService, TenantService>();
        services.AddTransient<ISessionService, SessionService>();
    }

    static void AddProcessings(this IServiceCollection services)
    {
        services.AddTransient<ISSOUserProcessingService, SSOUserProcessingService>();
        services.AddTransient<ISSOPrivilegeProcessingService, SSOPrivilegeProcessingService>();
        services.AddTransient<ISSOUserRoleProcessingService, SSOUserRoleProcessingService>();
        services.AddTransient<ISSORoleProcessingService, SSORoleProcessingService>();
        services.AddTransient<ITokenProcessingService, TokenProcessingService>();
        services.AddTransient<ITenantProcessingService, TenantProcessingService>();
        services.AddTransient<ISessionProcessingService, SessionProcessingService>();
    }

    static void AddOrchestrations(this IServiceCollection services)
    {
        services.AddTransient<ISSOUserOrchestrationService, SSOUserRegistrationOrchestrationService>();
        services.AddTransient<ISSOAuthInfoOrchestrationService, SSOAuthInfoOrchestrationService>();
        services.AddTransient<IAuthenticationOrchestrationService, AuthenticationOrchestrationService>();
    }
}
