using Security.Data;
using Security.Data.Brokers.Encryption;
using Security.Data.Interfaces;
using Security.Objects;

namespace Security.Api
{
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
}

