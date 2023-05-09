using Newtonsoft.Json;

namespace Axendo.Umbraco.Forms.Pipedrive.Core.Models.Responses
{
    public class PersonResponse
    {
        [JsonProperty(PropertyName = "data")]
        public Person Person { get; set; } = new();
    }
}
