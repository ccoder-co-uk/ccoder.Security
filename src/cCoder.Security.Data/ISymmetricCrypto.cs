// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Data;

public interface ISymmetricCrypto<T>
{
    string Encrypt(T source, string key);
    string Encrypt(T source);

    T Decrypt(string source, string key);
    T Decrypt(string source);
}