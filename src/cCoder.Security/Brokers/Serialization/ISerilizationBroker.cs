// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Brokers.Serialization;

internal interface ISerializationBroker
{
    T Deserialize<T>(string input);
    string Serialize(object obj);
}