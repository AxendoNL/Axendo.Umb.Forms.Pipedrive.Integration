using Newtonsoft.Json;

namespace Axendo.Umbraco.Forms.Pipedrive.Core.Models.Responses
{
    public class PersonItem
    {
        [JsonProperty("item")]
        public Person Person { get; set; } = new();
    }
}
