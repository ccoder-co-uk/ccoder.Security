// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Exposures.EDM;
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
        this IServiceCollection newIServiceCollection,
        Action<IServiceCollection, SecurityConfiguration> configAction) =>
        newIServiceCollection.AddSecurityWeb(configAction: configAction);

    public static SecurityConfiguration AddSecurityWeb(
        this IServiceCollection newIServiceCollection,
        Action<IServiceCollection, SecurityConfiguration> configAction) =>
        newIServiceCollection.AddSecurity(configAction: configAction);

    public static SecurityConfiguration AddSecurityHostedServices(
        this IServiceCollection newIServiceCollection,
        Action<IServiceCollection, SecurityConfiguration> configAction)
    {
        SecurityConfiguration securityConfiguration = newIServiceCollection.AddSecurity(configAction: configAction);
        newIServiceCollection.AddSecurityHostedServiceExposures();

        return securityConfiguration;
    }

    public static SecurityConfiguration AddSecurity(
        this IServiceCollection newIServiceCollection,
        Action<IServiceCollection, SecurityConfiguration> configAction)
    {
        SecurityConfiguration securityConfiguration = new();
        configAction(arg1: newIServiceCollection, arg2: securityConfiguration);
        newIServiceCollection.AddSingleton(implementationInstance: securityConfiguration);

        newIServiceCollection.AddEventing();
        newIServiceCollection.AddEventingTypes();
        newIServiceCollection.AddAspNet();
        newIServiceCollection.AddBrokers();
        newIServiceCollection.AddFoundations();
        newIServiceCollection.AddProcessings();
        newIServiceCollection.AddOrchestrations();
        newIServiceCollection.AddExposures();
        newIServiceCollection.AddEventHandlers();

        if (!string.IsNullOrWhiteSpace(value: securityConfiguration.RootPath))
        { newIServiceCollection.AddSecurityApiLayer(atPath: securityConfiguration.RootPath); }

        return securityConfiguration;
    }

    private static void AddEventingTypes(this IServiceCollection newIServiceCollection)
    {
        newIServiceCollection.AddEventingForType<SetupDetails>();
        newIServiceCollection.AddEventingForType<SecurityAccountEvent>();
    }

    private static void AddBrokers(this IServiceCollection newIServiceCollection)
    {
        newIServiceCollection.AddTransient<IHttpRequestBroker, HttpRequestBroker>();
        newIServiceCollection.AddTransient<ISessionBroker, SessionBroker>();
        newIServiceCollection.AddTransient<ISSOPrivilegeBroker, SSOPrivilegeBroker>();
        newIServiceCollection.AddTransient<ISSORoleBroker, SSORoleBroker>();
        newIServiceCollection.AddTransient<ISSOUserBroker, SSOUserBroker>();
        newIServiceCollection.AddTransient<ISSOUserRoleBroker, SSOUserRoleBroker>();
        newIServiceCollection.AddTransient<ITenantBroker, TenantBroker>();
        newIServiceCollection.AddTransient<ITenantAnalysisBroker, TenantAnalysisBroker>();
        newIServiceCollection.AddTransient<ITokenBroker, TokenBroker>();
        newIServiceCollection.AddTransient<IUserEventBroker, UserEventBroker>();
        newIServiceCollection.AddTransient<ISerializationBroker, SerializationBroker>();
        newIServiceCollection.AddTransient<ISecurityDateTimeOffsetBroker, SecurityDateTimeOffsetBroker>();
        newIServiceCollection.AddTransient<ISSOAuthorizationBroker, SSOAuthorizationBroker>();

        newIServiceCollection.AddTransient<IEventHubBroker, EventHubBroker>();
        newIServiceCollection.AddTransient<IAccountEventBroker, AccountEventBroker>();
        newIServiceCollection.AddTransient<ITenantSetupEventBroker, TenantSetupEventBroker>();
    }

    private static void AddFoundations(this IServiceCollection newIServiceCollection)
    {
        newIServiceCollection.AddTransient(implementationFactory: async provider =>
            await provider
                .GetRequiredService<ISSOAuthInfoOrchestrationService>()
                .GetSSOAuthInfoAsync());

        newIServiceCollection.AddTransient(implementationFactory: provider =>
        {
            Task<ISSOAuthInfo> authInfoTask = provider.GetRequiredService<Task<ISSOAuthInfo>>();
            authInfoTask.Wait();
            return authInfoTask.Result;
        });

        newIServiceCollection.AddTransient<ISSOUserService, SSOUserService>();
        newIServiceCollection.AddTransient<ISSOPrivilegeService, SSOPrivilegeService>();
        newIServiceCollection.AddTransient<ISSOUserRoleService, SSOUserRoleService>();
        newIServiceCollection.AddTransient<ISSORoleService, SSORoleService>();
        newIServiceCollection.AddTransient<ITokenService, TokenService>();
        newIServiceCollection.AddTransient<ITenantService, TenantService>();
        newIServiceCollection.AddTransient<ITenantAnalysisService, TenantAnalysisService>();
        newIServiceCollection.AddTransient<ISessionService, SessionService>();
        newIServiceCollection.AddTransient<IUserEventService, UserEventService>();

        newIServiceCollection.AddTransient<IEventHandlerService, EventHandlerService>();
        newIServiceCollection.AddTransient<IAccountEventService, AccountEventService>();
        newIServiceCollection.AddTransient<ITenantSetupEventService, TenantSetupEventService>();
    }

    private static void AddProcessings(this IServiceCollection newIServiceCollection)
    {
        newIServiceCollection.AddTransient<ISSOUserProcessingService, SSOUserProcessingService>();
        newIServiceCollection.AddTransient<ISSOPrivilegeProcessingService, SSOPrivilegeProcessingService>();
        newIServiceCollection.AddTransient<ISSOUserRoleProcessingService, SSOUserRoleProcessingService>();
        newIServiceCollection.AddTransient<ISSORoleProcessingService, SSORoleProcessingService>();
        newIServiceCollection.AddTransient<ITokenProcessingService, TokenProcessingService>();
        newIServiceCollection.AddTransient<ITenantProcessingService, TenantProcessingService>();
        newIServiceCollection.AddTransient<ITenantAnalysisProcessingService, TenantAnalysisProcessingService>();
        newIServiceCollection.AddTransient<ISessionProcessingService, SessionProcessingService>();
        newIServiceCollection.AddTransient<IUserEventProcessingService, UserEventProcessingService>();

        newIServiceCollection.AddTransient<ITenantSetupEventProcessingService, TenantSetupEventProcessingService>();
    }

    private static void AddOrchestrations(this IServiceCollection newIServiceCollection)
    {
        newIServiceCollection.AddTransient<ISSOAuthInfoOrchestrationService, SSOAuthInfoOrchestrationService>();
        newIServiceCollection.AddTransient<IAuthenticationOrchestrationService, AuthenticationOrchestrationService>();
        newIServiceCollection.AddTransient<ITenantOrchestrationService, TenantOrchestrationService>();
        newIServiceCollection.AddTransient<ITenantRelationsOrchestrationService, TenantRelationsOrchestrationService>();
        newIServiceCollection.AddTransient<ITenantSetupOrchestrationService, TenantSetupOrchestrationService>();
        newIServiceCollection.AddTransient<ITenantCoordinationService, TenantCoordinationService>();
        newIServiceCollection.AddTransient<ISSOUserOrchestrationService, SSOUserOrchestrationService>();
        newIServiceCollection.AddTransient<ISSOUserRoleOrchestrationService, SSOUserRoleOrchestrationService>();
        newIServiceCollection.AddTransient<ISSORoleOrchestrationService, SSORoleOrchestrationService>();
    }

    private static void AddExposures(this IServiceCollection newIServiceCollection)
    {
        newIServiceCollection.AddTransient<ITokenManager, TokenManager>();
        newIServiceCollection.AddTransient<ITenantManager, TenantManager>();
    }

    private static void AddSecurityHostedServiceExposures(this IServiceCollection newIServiceCollection)
    {
        newIServiceCollection.AddSingleton<ITokenCleaner, TokenCleaner>();

        newIServiceCollection.AddSingleton<IHostedService>(implementationFactory: serviceProvider =>
            serviceProvider.GetRequiredService<ITokenCleaner>());
    }

    private static void AddEventHandlers(this IServiceCollection newIServiceCollection) =>
        newIServiceCollection.AddTransient<ISecurityEventHandlers, SecurityEventHandlers>();

    private static void AddAspNet(this IServiceCollection newIServiceCollection)
    {
        newIServiceCollection.AddHttpContextAccessor();
        newIServiceCollection.AddTransient(implementationFactory: ctx => ctx.GetService<IHttpContextAccessor>()?.HttpContext);
        newIServiceCollection.AddTransient(implementationFactory: ctx => ctx.GetService<HttpContext>()?.Request);
        newIServiceCollection.AddTransient(implementationFactory: ctx => ctx.GetService<HttpContext>()?.Session);
        newIServiceCollection.AddSession();
    }

    public static void AddSecurityApiLayer(this IServiceCollection newIServiceCollection, string atPath) =>
        newIServiceCollection.AddControllers()
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
