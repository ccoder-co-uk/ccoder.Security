using Microsoft.AspNetCore.Identity;

namespace cCoder.Security.Brokers.Encryption;
internal class PasswordEncryptionBrokerHasher : IPasswordEncryptionBroker
{
    private readonly PasswordHasher<object> hasher;

    public PasswordEncryptionBrokerHasher() =>
        this.hasher = new PasswordHasher<object>();

    public string Encrypt(string password) =>
        hasher.HashPassword(new object(), password);

    public bool EncryptedAndPlainTextAreEqual(string encrypted, string plainText) =>
        hasher.VerifyHashedPassword(new object(), encrypted, plainText) is
            PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
}



