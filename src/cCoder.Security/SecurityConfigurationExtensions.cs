using cCoder.Security.Data;
using cCoder.Security.Brokers.Encryption;
using cCoder.Security.Objects;
using System.Security.Cryptography;

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
        {
            decryptionKey = GenerateHexKey(32);
        }

        services.AddTransient<ISymmetricCrypto<string>>(_ => new AesCrypto<string>(decryptionKey));
        services.AddTransient<IPasswordEncryptionBroker, PasswordEncryptionBrokerAESHMAC>();
    }

    private static string GenerateHexKey(int bytes)
    {
        using var rng = RandomNumberGenerator.Create();
        var data = new byte[bytes];
        rng.GetBytes(data);

        return BitConverter.ToString(data)
            .Replace("-", "");
    }

    public static void UsePasswordHasherHashing(
        this SecurityConfiguration config,
        IServiceCollection services) =>
            services.AddTransient<IPasswordEncryptionBroker, PasswordEncryptionBrokerHasher>();
}


