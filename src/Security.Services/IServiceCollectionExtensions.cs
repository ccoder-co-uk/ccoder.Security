using Microsoft.Extensions.DependencyInjection;
using Security.Data.Brokers.Authentication;
using Security.Data.Brokers.DateTime;
using Security.Data.Brokers.Requests;
using Security.Data.Brokers.Serialization;
using Security.Data.Brokers.Storage;
using Security.Data.Brokers.Storage.Interfaces;
using Security.Data.EF;
using Security.Data.EF.Interfaces;
using Security.Objects;
using Security.Services.Foundation;
using Security.Services.Foundation.Interfaces;
using Security.Services.Orchestration;
using Security.Services.Orchestration.Interfaces;
using Security.Services.Processing;
using Security.Services.Processing.Interfaces;
using System;

namespace Security.Services;

public static class IServiceCollectionExtensions
{
    public static SecurityConfiguration AddSecurityServices(
        this IServiceCollection services,
        Action<IServiceCollection, SecurityConfiguration> configAction)
    {
        var securityConfiguration = new SecurityConfiguration();
        configAction(services, securityConfiguration);

        services.AddTransient<ISSODbContextFactory, SSODbContextFactory>();

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

        services.AddTransient<IIdentityBroker, IdentityBroker>();
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
