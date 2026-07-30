// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Authentication;
using cCoder.Security.Dependencies.Sessions;
using cCoder.Security.Brokers.Configuration;
using cCoder.Security.Brokers.Events;
using cCoder.Security.Brokers.DateTime;
using cCoder.Security.Brokers.Requests;
using cCoder.Security.Brokers.Logging;
using cCoder.Security.Brokers.Serialization;
using cCoder.Security.Brokers.Sessions;
using cCoder.Security.Brokers.Storage;
using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Brokers.Utility;
using cCoder.Security.Brokers.Utility.Interfaces;
using cCoder.Security.Data.Models;
using cCoder.Security.Data.EF;
using cCoder.Security.Exposures;
using cCoder.Security.Exposures.EventHandlers;
using cCoder.Security.Exposures.HostedServices;
using cCoder.Security.Dependencies.HostedServices;
using cCoder.Security.Models;
using cCoder.Security.Models.Configurations;
using cCoder.Security.Models.Events;
using cCoder.Security.Services.Foundations;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Foundations.Events;
using cCoder.Security.Services.Orchestrations;
using cCoder.Security.Services.Orchestrations.Interfaces;
using cCoder.Security.Services.Processings;
using cCoder.Security.Services.Processings.Interfaces;
using cCoder.Security.Services.Aggregations;
using cCoder.Security.Services.Aggregations.Interfaces;
using cCoder.Eventing;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;

namespace cCoder.Security;

public static class IServiceCollectionExtensions
{
    public static void AddSecurityWeb(
        this IServiceCollection services,
        Action<SecurityConfiguration> configure,
        ODataConventionModelBuilder builder = null)
    {
        SecurityConfiguration configuration = new();
        configure?.Invoke(configuration);
        services.AddSecurityWeb(
            configuration: configuration,
            builder: builder);
    }

    public static void AddSecurityWeb(
        this IServiceCollection services,
        SecurityConfiguration configuration,
        ODataConventionModelBuilder builder = null)
    {
        ArgumentNullException.ThrowIfNull(argument: configuration);

        services.AddDependencies(configuration);
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddExposures();

        if (!string.IsNullOrWhiteSpace(value: configuration.RootPath))
        {
            services.AddSecurityApiLayer(
                atPath: configuration.RootPath,
                aggregateModelBuilder: builder);
        }
    }

    public static void AddSecurityHostedServices(
        this IServiceCollection services,
        Action<SecurityConfiguration> configure)
    {
        SecurityConfiguration configuration = new();
        configure?.Invoke(configuration);
        services.AddSecurityHostedServices(configuration: configuration);
    }

    public static void AddSecurityHostedServices(
        this IServiceCollection services,
        SecurityConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(argument: configuration);

        services.AddDependencies(configuration);
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddExposures();
        services.AddHostedDependencies();

        if (!string.IsNullOrWhiteSpace(value: configuration.RootPath))
        {
            services.AddSecurityApiLayer(atPath: configuration.RootPath);
        }
    }

    private static void AddDependencies(
        this IServiceCollection services,
        SecurityConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(argument: configuration);

        if (!string.IsNullOrWhiteSpace(value: configuration.ConnectionString))
        {
            services.AddSecurityData(configuration);
        }

        if (!string.IsNullOrWhiteSpace(value: configuration.DecryptionKey))
        {
            configuration.UseAESHMMACPasswordEncryption(
                services: services,
                decryptionKey: configuration.DecryptionKey);
        }

        services.AddSingleton(implementationInstance: configuration);

        services.AddEventing();
        services.AddEventingTypes();
        services.AddAspNet();
        services.AddEventHandlers();
    }

    private static void AddEventingTypes(this IServiceCollection services)
    {
        services.AddEventingForType<SetupDetails>();
        services.AddEventingForType<SecurityAccountEvent>();
    }

    private static void AddBrokers(this IServiceCollection services)
    {
        services.AddSingleton<ISecurityConfigurationBroker, SecurityConfigurationBroker>();
        services.AddTransient<IAuthenticationContextBroker, AuthenticationContextBroker>();
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
        services.AddTransient<IAuthorizationService, AuthorizationService>();
        services.AddTransient<IAuthorizationProcessingService, AuthorizationProcessingService>();
        services.AddTransient<IRequestService, RequestService>();
        services.AddTransient<IRequestProcessingService, RequestProcessingService>();
        services.AddTransient<ILoggingBroker, LoggingBroker>();
        services.AddTransient<ILoggingService, LoggingService>();
        services.AddTransient<ILoggingProcessingService, LoggingProcessingService>();
        services.AddTransient<IAccountEventProcessingService, AccountEventProcessingService>();

        services.AddTransient<IEventHubBroker, EventHubBroker>();
        services.AddTransient<IAccountEventBroker, AccountEventBroker>();
        services.AddTransient<ITenantSetupEventBroker, TenantSetupEventBroker>();
    }

    private static void AddFoundations(this IServiceCollection services)
    {
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
        services.AddTransient<ISSOPrivilegeManager, SSOPrivilegeProcessingService>();
        services.AddTransient<ISSOUserRoleProcessingService, SSOUserRoleProcessingService>();
        services.AddTransient<ISSORoleProcessingService, SSORoleProcessingService>();
        services.AddTransient<ITokenProcessingService, TokenProcessingService>();
        services.AddTransient<ITenantProcessingService, TenantProcessingService>();
        services.AddTransient<ITenantAnalysisProcessingService, TenantAnalysisProcessingService>();
        services.AddTransient<ITenantAnalysisManager, TenantAnalysisProcessingService>();
        services.AddTransient<ISessionProcessingService, SessionProcessingService>();
        services.AddTransient<IUserEventProcessingService, UserEventProcessingService>();
        services.AddTransient<IUserEventManager, UserEventProcessingService>();

    }

    private static void AddOrchestrations(this IServiceCollection services)
    {
        services.AddTransient<ISSOAuthInfoAggregationService, SSOAuthInfoAggregationService>();
        services.AddTransient<IAuthenticationAggregationService, AuthenticationAggregationService>();
        services.AddTransient<IAuthenticationManager, AuthenticationAggregationService>();
        services.AddTransient<ICurrentUserAggregationService, CurrentUserAggregationService>();
        services.AddTransient<ISecurityCurrentUserManager, CurrentUserAggregationService>();
        services.AddTransient<ITenantAggregationService, TenantAggregationService>();
        services.AddTransient<ITenantAdministrationManager, TenantAggregationService>();
        services.AddTransient<ISSOUserAggregationService, SSOUserAggregationService>();
        services.AddTransient<ISSOUserManager, SSOUserAggregationService>();
        services.AddTransient<IRegistrationAggregationService, RegistrationAggregationService>();
        services.AddTransient<IRegistrationManager, RegistrationAggregationService>();
        services.AddTransient<ISSOUserRoleOrchestrationService, SSOUserRoleOrchestrationService>();
        services.AddTransient<ISSOUserRoleManager, SSOUserRoleOrchestrationService>();
        services.AddTransient<ISSORoleOrchestrationService, SSORoleOrchestrationService>();
        services.AddTransient<ISSORoleManager, SSORoleOrchestrationService>();
    }

    private static void AddExposures(this IServiceCollection services)
    {
        services.AddTransient(implementationFactory: async provider =>
            await provider
                .GetRequiredService<ISSOAuthInfoAggregationService>()
                .GetSSOAuthInfoAsync());

        services.AddTransient(implementationFactory: provider =>
        {
            Task<ISSOAuthInfo> authInfoTask = provider.GetRequiredService<Task<ISSOAuthInfo>>();
            authInfoTask.Wait();
            return authInfoTask.Result;
        });

        services.AddTransient<ITokenManager, TokenManager>();
        services.AddTransient<ITenantManager, TenantManager>();
    }

    private static void AddHostedDependencies(this IServiceCollection services)
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
        services.AddTransient<ISession>(implementationFactory: context =>
            context.GetService<HttpContext>()?.Session ??
            new NullSession());
        services.AddSession();
    }

    private static void AddSecurityApiLayer(
        this IServiceCollection services,
        string atPath,
        ODataConventionModelBuilder aggregateModelBuilder = null)
    {
        ODataConventionModelBuilder modelBuilder = new();
        modelBuilder.ConfigureSecurityApiModel();
        aggregateModelBuilder?.ConfigureSecurityApiModel();

        IMvcBuilder mvcBuilder = services.AddControllers();
        mvcBuilder.AddOData(setupAction: options =>
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
                    model: modelBuilder.GetEdmModel());
            });
    }
}
