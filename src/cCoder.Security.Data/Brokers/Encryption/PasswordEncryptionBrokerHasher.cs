using Microsoft.AspNet.Identity;

namespace cCoder.Security.Data.Brokers.Encryption;

public class PasswordEncryptionBrokerHasher : IPasswordEncryptionBroker
{
    private readonly PasswordHasher hasher;

    public PasswordEncryptionBrokerHasher() =>
        this.hasher = new PasswordHasher();

    public string Encrypt(string password) =>
        hasher.HashPassword(password);

    public bool EncryptedAndPlainTextAreEqual(string encrypted, string plainText) =>
        hasher.VerifyHashedPassword(encrypted, plainText) is
            PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
}

