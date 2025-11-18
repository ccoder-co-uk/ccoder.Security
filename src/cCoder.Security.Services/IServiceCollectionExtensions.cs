using cCoder.Security.Data.Brokers.DateTime;
using cCoder.Security.Data.Brokers.Requests;
using cCoder.Security.Data.Brokers.Serialization;
using cCoder.Security.Data.Brokers.Storage;
using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Data.Brokers.Utility;
using cCoder.Security.Data.Brokers.Utility.Interfaces;
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
        SecurityConfiguration securityConfiguration = new();
        configAction(services, securityConfiguration);

        services.AddScoped(async provider => await provider.GetService<ISSOAuthInfoOrchestrationService>().GetSSOAuthInfoAsync());

        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddCoordinations();

        return securityConfiguration;
    }

    private static void AddBrokers(this IServiceCollection services)
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
        services.AddScoped<ISSOAuthorizationBroker, SSOAuthorizationBroker>();
    }

    private static void AddFoundations(this IServiceCollection services)
    {
        services.AddScoped<ISSOUserService, SSOUserService>();
        services.AddScoped<ISSOPrivilegeService, SSOPrivilegeService>();
        services.AddScoped<ISSOUserRoleService, SSOUserRoleService>();
        services.AddScoped<ISSORoleService, SSORoleService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<ITenantAnalysisService, TenantAnalysisService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IUserEventService, UserEventService>();
    }

    private static void AddProcessings(this IServiceCollection services)
    {
        services.AddScoped<ISSOUserProcessingService, SSOUserProcessingService>();
        services.AddScoped<ISSOPrivilegeProcessingService, SSOPrivilegeProcessingService>();
        services.AddScoped<ISSOUserRoleProcessingService, SSOUserRoleProcessingService>();
        services.AddScoped<ISSORoleProcessingService, SSORoleProcessingService>();
        services.AddScoped<ITokenProcessingService, TokenProcessingService>();
        services.AddScoped<ITenantProcessingService, TenantProcessingService>();
        services.AddScoped<ITenantAnalysisProcessingService, TenantAnalysisProcessingService>();
        services.AddScoped<ISessionProcessingService, SessionProcessingService>();
        services.AddScoped<IUserEventProcessingService, UserEventProcessingService>();
    }

    private static void AddOrchestrations(this IServiceCollection services)
    {
        services.AddScoped<ISSOAuthInfoOrchestrationService, SSOAuthInfoOrchestrationService>();
        services.AddScoped<IAuthenticationOrchestrationService, AuthenticationOrchestrationService>();
        services.AddScoped<ITenantOrchestrationService, TenantOrchestrationService>();
        services.AddScoped<ITenantRelationsOrchestrationService, TenantRelationsOrchestrationService>();

        services.AddScoped<ISSOUserOrchestrationService, SSOUserRegistrationOrchestrationService>();
        services.AddScoped<ISSOUserRoleOrchestrationService, SSOUserRoleOrchestrationService>();
        services.AddScoped<ISSORoleOrchestrationService, SSORoleOrchestrationService>();
    }

    private static void AddCoordinations(this IServiceCollection services)
    {
        services.AddScoped<ITenantCoordinationService, TenantCoordinationService>();
    }
}
