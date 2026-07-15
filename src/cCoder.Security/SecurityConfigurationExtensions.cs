using cCoder.Security.Data;
using cCoder.Security.Brokers.Encryption;
using cCoder.Security.Objects;
using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations;

namespace cCoder.Security;

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
        if (string.IsNullOrEmpty(decryptionKey))
            throw new ValidationException("Decryption key cannot be empty");

        services.AddTransient<ISymmetricCrypto<string>>(_ => new AesCrypto<string>(decryptionKey));
        services.AddTransient<IPasswordEncryptionBroker, PasswordEncryptionBrokerAESHMAC>();
    }
    
    public static void UsePasswordHasherHashing(
        this SecurityConfiguration config,
        IServiceCollection services) =>
            services.AddTransient<IPasswordEncryptionBroker, PasswordEncryptionBrokerHasher>();
}


