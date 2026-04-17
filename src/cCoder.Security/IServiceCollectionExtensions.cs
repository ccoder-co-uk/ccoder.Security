using cCoder.Security.Api.EDM;
using cCoder.Security.Api;
using cCoder.Security.Brokers.Events;
using cCoder.Security.Brokers.DateTime;
using cCoder.Security.Brokers.Requests;
using cCoder.Security.Brokers.Serialization;
using cCoder.Security.Brokers.Storage;
using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Brokers.Utility;
using cCoder.Security.Brokers.Utility.Interfaces;
using cCoder.Security.Data.Models;
using cCoder.Security.Exposures;
using cCoder.Security.Exposures.EventHandlers;
using cCoder.Security.Objects;
using cCoder.Security.Services.Foundations;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Foundations.Events;
using cCoder.Security.Services.Orchestrations;
using cCoder.Security.Services.Orchestrations.Interfaces;
using cCoder.Security.Services.Processings;
using cCoder.Security.Services.Processings.Events;
using cCoder.Security.Services.Processings.Interfaces;
using EventLibrary;
using Microsoft.AspNetCore.OData;

namespace cCoder.Security;

public static class IServiceCollectionExtensions
{
    public static void AddSecurityApi(
        this IServiceCollection services,
        Action<IServiceCollection, SecurityConfiguration> configAction) =>
        services.AddSecurity(configAction);

    public static SecurityConfiguration AddSecurity(
        this IServiceCollection services,
        Action<IServiceCollection, SecurityConfiguration> configAction)
    {
        SecurityConfiguration securityConfiguration = new();
        configAction(services, securityConfiguration);

        services.AddEventing();
        services.AddEventingTypes();
        services.AddAspNet();
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddExposures();
        services.AddEventHandlers();

        if (!string.IsNullOrWhiteSpace(securityConfiguration.RootPath))
            services.AddSecurityApiLayer(securityConfiguration.RootPath);

        return securityConfiguration;
    }

    private static void AddEventingTypes(this IServiceCollection services) =>
        services.AddEventingForType<SetupDetails>();

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

        services.AddTransient<IEventHubBroker, EventHubBroker>();
        services.AddTransient<ITenantSetupEventBroker, TenantSetupEventBroker>();
    }

    private static void AddFoundations(this IServiceCollection services)
    {
        services.AddScoped(async provider =>
            await provider.GetRequiredService<ISSOAuthInfoOrchestrationService>().GetSSOAuthInfoAsync());

        services.AddScoped(provider =>
        {
            Task<ISSOAuthInfo> authInfoTask = provider.GetRequiredService<Task<ISSOAuthInfo>>();
            authInfoTask.Wait();
            return authInfoTask.Result;
        });

        services.AddScoped<ISSOUserService, SSOUserService>();
        services.AddScoped<ISSOPrivilegeService, SSOPrivilegeService>();
        services.AddScoped<ISSOUserRoleService, SSOUserRoleService>();
        services.AddScoped<ISSORoleService, SSORoleService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<ITenantAnalysisService, TenantAnalysisService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IUserEventService, UserEventService>();

        services.AddTransient<IEventHandlerService, EventHandlerService>();
        services.AddTransient<ITenantSetupEventService, TenantSetupEventService>();
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

        services.AddTransient<ITenantSetupEventProcessingService, TenantSetupEventProcessingService>();
    }

    private static void AddOrchestrations(this IServiceCollection services)
    {
        services.AddScoped<ISSOAuthInfoOrchestrationService, SSOAuthInfoOrchestrationService>();
        services.AddScoped<IAuthenticationOrchestrationService, AuthenticationOrchestrationService>();
        services.AddScoped<ITenantOrchestrationService, TenantOrchestrationService>();
        services.AddScoped<ITenantRelationsOrchestrationService, TenantRelationsOrchestrationService>();
        services.AddScoped<ITenantSetupOrchestrationService, TenantSetupOrchestrationService>();
        services.AddScoped<ITenantCoordinationService, TenantCoordinationService>();
        services.AddScoped<ISSOUserOrchestrationService, SSOUserRegistrationOrchestrationService>();
        services.AddScoped<ISSOUserRoleOrchestrationService, SSOUserRoleOrchestrationService>();
        services.AddScoped<ISSORoleOrchestrationService, SSORoleOrchestrationService>();
    }

    private static void AddExposures(this IServiceCollection services)
    {
        services.AddTransient<IAccountManager, AccountManager>();
        services.AddTransient<ITenantManager, TenantManager>();
    }

    private static void AddEventHandlers(this IServiceCollection services) =>
        services.AddTransient<ISecurityEventHandlers, SecurityEventHandlers>();

    private static void AddAspNet(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddTransient(ctx => ctx.GetService<IHttpContextAccessor>()?.HttpContext);
        services.AddTransient(ctx => ctx.GetService<HttpContext>()?.Request);
        services.AddTransient(ctx => ctx.GetService<HttpContext>()?.Session);
        services.AddSession();
    }

    public static void AddSecurityApiLayer(this IServiceCollection services, string atPath) =>
        services.AddControllers()
            .AddOData(options =>
            {
                options.Expand().Count().Filter().Select().OrderBy().SetMaxTop(1000);
                options.AddRouteComponents(atPath, new SecurityModelBuilder().Build().EDMModel);
            });
}


