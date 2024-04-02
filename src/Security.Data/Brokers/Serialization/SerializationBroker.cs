using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Security.Data.Brokers.Serialization
{
    public class SerializationBroker : ISerializationBroker
	{
        public static readonly JsonSerializerSettings ODataJsonSettings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None,
            Formatting = Formatting.None,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            NullValueHandling = NullValueHandling.Ignore,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            ContractResolver = new DefaultContractResolver { IgnoreSerializableAttribute = true },
            MaxDepth = 4
        };

		public T Deserialize<T>(string input) => 
			JsonConvert.DeserializeObject<T>(input);

		public string Serialize(object input) =>
            JsonConvert.SerializeObject(input, Formatting.None, ODataJsonSettings);
    }
}