// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Exposures;

namespace cCoder.Security.Brokers.Encoding;

internal interface IEncodingBroker : IUtilityBroker
{
    string GetString(byte[] bytes);
}