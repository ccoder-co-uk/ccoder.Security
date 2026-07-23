// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security;
using cCoder.Security.Data.EF;

namespace Security.HostedServices;

public static class IServiceCollectionExtensions
{
    public static void AddSecurityHostedServicesApplication(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services.AddSecurityHostedServices(
            configAction: (serviceCollection, securityConfig) =>
            {
                securityConfig.AddMSSQLModelProvider(
                    services: serviceCollection,
                    connectionString: configuration.GetConnectionString("SSO"));

                securityConfig.UseAESHMMACPasswordEncryption(
                    services: serviceCollection,
                    decryptionKey: configuration
                        .GetSection("settings")["DecryptionKey"]);

                securityConfig.IsMigrating =
                    configuration.GetValue<int?>(key: "MIGRATING") == 1 ||
                    configuration.GetValue<bool?>(
                        key: "Security:IsMigrating") == true;
            });
}