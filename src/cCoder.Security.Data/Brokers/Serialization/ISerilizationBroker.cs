using System;
namespace cCoder.Security.Data.Brokers.Serialization
{
	public interface ISerializationBroker
	{
        T Deserialize<T>(string input);
        string Serialize(object obj);
    }
}

