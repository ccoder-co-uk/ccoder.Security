using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Security.AcceptanceTests.Clients;

public class ODataCollection<TCollectionType>
{
    [JsonProperty("@odata.context")]
    public string ODataContext { get; set; }

    public IEnumerable<TCollectionType> Value { get; set; }
}