namespace cCoder.Security.Brokers.Encryption;
internal interface IPasswordEncryptionBroker
{
    string Encrypt(string password);
    bool EncryptedAndPlainTextAreEqual(string encrypted, string plainText);
}

