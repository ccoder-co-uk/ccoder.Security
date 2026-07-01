using System.Security.Cryptography;
using System.Text;

namespace cCoder.Security.Brokers.Encryption;
internal class PasswordEncryptionBrokerSHA512 : IPasswordEncryptionBroker
{
    public string Encrypt(string password)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(password);
        byte[] hashBytes = SHA512.HashData(inputBytes);
        return Encoding.UTF8.GetString(hashBytes);
    }

    public bool EncryptedAndPlainTextAreEqual(string encrypted, string plainText)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] hashBytes = SHA512.HashData(inputBytes);
        string hashedString = Encoding.UTF8.GetString(hashBytes);
        return hashedString == encrypted;
    }
}
