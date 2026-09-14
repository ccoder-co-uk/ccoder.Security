// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Exposures;

namespace cCoder.Security.Brokers.Serialization;

internal interface ISerializationBroker : IUtilityBroker
{
    T Deserialize<T>(string input);
    string Serialize(object obj);
}