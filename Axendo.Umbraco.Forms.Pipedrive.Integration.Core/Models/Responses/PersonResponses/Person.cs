using Newtonsoft.Json;

namespace Axendo.Umbraco.Forms.Pipedrive.Core.Models.Responses
{
    public class Person
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("primary_email")]
        public string Email { get; set; } = string.Empty;
    }
}
