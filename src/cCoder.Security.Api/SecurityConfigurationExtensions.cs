using cCoder.Security.Data;
using cCoder.Security.Data.Brokers.Encryption;
using cCoder.Security.Data.Interfaces;
using cCoder.Security.Objects;

namespace cCoder.Security.Api;

public static class SecurityConfigurationExtensions
{
    public static void UseSHA512PasswordEncryption(
        this SecurityConfiguration config,
        IServiceCollection services) =>
            services.AddTransient<IPasswordEncryptionBroker, PasswordEncryptionBrokerSHA512>();

    public static void UseAESHMMACPasswordEncryption(
        this SecurityConfiguration config,
        IServiceCollection services,
        string decryptionKey)
    {
        services.AddTransient<ISymmetricCrypto<string>>(_ => new AesCrypto<string>(decryptionKey));
        services.AddTransient<IPasswordEncryptionBroker, PasswordEncryptionBrokerAESHMAC>();
    }
}

