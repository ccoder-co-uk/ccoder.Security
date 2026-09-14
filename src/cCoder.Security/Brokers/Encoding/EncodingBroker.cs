// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Brokers.Encoding;

internal sealed class EncodingBroker : IEncodingBroker
{
    public string GetString(byte[] bytes) =>
        System.Text.Encoding.UTF8.GetString(bytes: bytes);
}