// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Exposures.EDM;
using cCoder.Security.Brokers.Configuration;
using cCoder.Security.Brokers.Events;
using cCoder.Security.Brokers.DateTime;
using cCoder.Security.Brokers.Requests;
using cCoder.Security.Brokers.Serialization;
using cCoder.Security.Brokers.Sessions;
using cCoder.Security.Brokers.Storage;
using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Brokers.Utility;
using cCoder.Security.Brokers.Utility.Interfaces;
using cCoder.Security.Data.Models;
using cCoder.Security.Exposures;
using cCoder.Security.Exposures.EventHandlers;
using cCoder.Security.Exposures.HostedServices;
using cCoder.Security.Objects;
using cCoder.Security.Objects.Events;
using cCoder.Security.Services.Foundations;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Foundations.Events;
using cCoder.Security.Services.Orchestrations;
using cCoder.Security.Services.Orchestrations.Interfaces;
using cCoder.Security.Services.Processings;
using cCoder.Security.Services.Processings.Events;
using cCoder.Security.Services.Processings.Interfaces;
using cCoder.Eventing;
using Microsoft.AspNetCore.OData;

namespace cCoder.Security;

public static class IServiceCollectionExtensions
{
    public static void AddSecurityApi(
        this IServiceCollection services,
        Action<IServiceCollection, SecurityConfiguration> configAction) =>
        services.AddSecurityWeb(configAction: configAction);

    public static SecurityConfiguration AddSecurityWeb(
        this IServiceCollection services,
        Action<IServiceCollection, SecurityConfiguration> configAction) =>
        services.AddSecurity(configAction: configAction);

    public static SecurityConfiguration AddSecurityHostedServices(
        this IServiceCollection services,
        Action<IServiceCollection, SecurityConfiguration> configAction)
    {
        SecurityConfiguration securityConfiguration = services.AddSecurity(configAction: configAction);
        services.AddSecurityHostedServiceExposures();

        return securityConfiguration;
    }

    public static SecurityConfiguration AddSecurity(
        this IServiceCollection services,
        Action<IServiceCollection, SecurityConfiguration> configAction)
    {
        SecurityConfiguration securityConfiguration = new();
        configAction(arg1: services, arg2: securityConfiguration);
        services.AddSingleton(implementationInstance: securityConfiguration);

        services.AddEventing();
        services.AddEventingTypes();
        services.AddAspNet();
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddExposures();
        services.AddEventHandlers();

        if (!string.IsNullOrWhiteSpace(value: securityConfiguration.RootPath))
        { services.AddSecurityApiLayer(atPath: securityConfiguration.RootPath); }

        return securityConfiguration;
    }

    private static void AddEventingTypes(this IServiceCollection services)
    {
        services.AddEventingForType<SetupDetails>();
        services.AddEventingForType<SecurityAccountEvent>();
    }

    private static void AddBrokers(this IServiceCollection services)
    {
        services.AddTransient<ISecurityConfigurationBroker, SecurityConfigurationBroker>();
        services.AddTransient<IWebSessionBroker, WebSessionBroker>();
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
        services.AddTransient<ISSOAuthorizationBroker, SSOAuthorizationBroker>();

        services.AddTransient<IEventHubBroker, EventHubBroker>();
        services.AddTransient<IAccountEventBroker, AccountEventBroker>();
        services.AddTransient<ITenantSetupEventBroker, TenantSetupEventBroker>();
    }

    private static void AddFoundations(this IServiceCollection services)
    {
        services.AddTransient(implementationFactory: async provider =>
            await provider
                .GetRequiredService<ISSOAuthInfoOrchestrationService>()
                .GetSSOAuthInfoAsync());

        services.AddTransient(implementationFactory: provider =>
        {
            Task<ISSOAuthInfo> authInfoTask = provider.GetRequiredService<Task<ISSOAuthInfo>>();
            authInfoTask.Wait();
            return authInfoTask.Result;
        });

        services.AddTransient<ISSOUserService, SSOUserService>();
        services.AddTransient<ISSOPrivilegeService, SSOPrivilegeService>();
        services.AddTransient<ISSOUserRoleService, SSOUserRoleService>();
        services.AddTransient<ISSORoleService, SSORoleService>();
        services.AddTransient<ITokenService, TokenService>();
        services.AddTransient<ITenantService, TenantService>();
        services.AddTransient<ITenantAnalysisService, TenantAnalysisService>();
        services.AddTransient<ISessionService, SessionService>();
        services.AddTransient<IUserEventService, UserEventService>();

        services.AddTransient<IEventHandlerService, EventHandlerService>();
        services.AddTransient<IAccountEventService, AccountEventService>();
        services.AddTransient<ITenantSetupEventService, TenantSetupEventService>();
    }

    private static void AddProcessings(this IServiceCollection services)
    {
        services.AddTransient<ISSOUserProcessingService, SSOUserProcessingService>();
        services.AddTransient<ISSOPrivilegeProcessingService, SSOPrivilegeProcessingService>();
        services.AddTransient<ISSOUserRoleProcessingService, SSOUserRoleProcessingService>();
        services.AddTransient<ISSORoleProcessingService, SSORoleProcessingService>();
        services.AddTransient<ITokenProcessingService, TokenProcessingService>();
        services.AddTransient<ITenantProcessingService, TenantProcessingService>();
        services.AddTransient<ITenantAnalysisProcessingService, TenantAnalysisProcessingService>();
        services.AddTransient<ISessionProcessingService, SessionProcessingService>();
        services.AddTransient<IUserEventProcessingService, UserEventProcessingService>();

        services.AddTransient<ITenantSetupEventProcessingService, TenantSetupEventProcessingService>();
    }

    private static void AddOrchestrations(this IServiceCollection services)
    {
        services.AddTransient<ISSOAuthInfoOrchestrationService, SSOAuthInfoOrchestrationService>();
        services.AddTransient<IAuthenticationOrchestrationService, AuthenticationOrchestrationService>();
        services.AddTransient<ITenantOrchestrationService, TenantOrchestrationService>();
        services.AddTransient<ITenantRelationsOrchestrationService, TenantRelationsOrchestrationService>();
        services.AddTransient<ITenantSetupOrchestrationService, TenantSetupOrchestrationService>();
        services.AddTransient<ITenantCoordinationService, TenantCoordinationService>();
        services.AddTransient<ISSOUserOrchestrationService, SSOUserOrchestrationService>();
        services.AddTransient<ISSOUserRoleOrchestrationService, SSOUserRoleOrchestrationService>();
        services.AddTransient<ISSORoleOrchestrationService, SSORoleOrchestrationService>();
    }

    private static void AddExposures(this IServiceCollection services)
    {
        services.AddTransient<ITokenManager, TokenManager>();
        services.AddTransient<ITenantManager, TenantManager>();
    }

    private static void AddSecurityHostedServiceExposures(this IServiceCollection services)
    {
        services.AddSingleton<ITokenCleaner, TokenCleaner>();

        services.AddSingleton<IHostedService>(implementationFactory: serviceProvider =>
            serviceProvider.GetRequiredService<ITokenCleaner>());
    }

    private static void AddEventHandlers(this IServiceCollection services) =>
        services.AddTransient<ISecurityEventHandlers, SecurityEventHandlers>();

    private static void AddAspNet(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddTransient(implementationFactory: ctx => ctx.GetService<IHttpContextAccessor>()?.HttpContext);
        services.AddTransient(implementationFactory: ctx => ctx.GetService<HttpContext>()?.Request);
        services.AddTransient(implementationFactory: ctx => ctx.GetService<HttpContext>()?.Session);
        services.AddSession();
    }

    public static void AddSecurityApiLayer(this IServiceCollection services, string atPath) =>
        services.AddControllers()
            .AddOData(setupAction: options =>
            {
                options
                    .Expand()
                    .Count()
                    .Filter()
                    .Select()
                    .OrderBy()
                    .SetMaxTop(maxTopValue: 1000);

                options.AddRouteComponents(
                    routePrefix: atPath,
                    model: new SecurityModelBuilder()
                        .Build()
                        .EDMModel);
            });
}