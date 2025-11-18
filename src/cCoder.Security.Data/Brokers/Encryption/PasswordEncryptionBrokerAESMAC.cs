namespace cCoder.Security.Data.Brokers.Encryption;

public class PasswordEncryptionBrokerAESHMAC(ISymmetricCrypto<string> crypto) 
    : IPasswordEncryptionBroker
{
    public string Encrypt(string password) => 
        crypto.Encrypt(password);

    public bool EncryptedAndPlainTextAreEqual(string encrypted, string plainText) => 
        crypto.Decrypt(encrypted) == plainText;
}
