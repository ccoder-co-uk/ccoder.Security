// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text.Json;

namespace cCoder.Security.Brokers.Serialization;

internal class SerializationBroker : ISerializationBroker
{
    private static readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public T Deserialize<T>(string input) =>
        JsonSerializer.Deserialize<T>(
            json: input,
            options: jsonSerializerOptions);

    public string Serialize(object input) =>
        JsonSerializer.Serialize(value: input);
}