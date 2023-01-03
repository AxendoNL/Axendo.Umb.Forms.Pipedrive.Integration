using Newtonsoft.Json;

namespace Axendo.Umb.Forms.Pipedrive.Web.Core.Models.Responses
{
    public class Person
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("primary_email")]
        public string Email { get; set; }

    }
}
