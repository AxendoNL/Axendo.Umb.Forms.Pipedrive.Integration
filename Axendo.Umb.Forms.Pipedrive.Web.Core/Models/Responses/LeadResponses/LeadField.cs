using Newtonsoft.Json;

namespace Axendo.Umb.Forms.Pipedrive.Web.Core.Models.Responses.LeadResponses
{
    public class LeadField
    {
        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}
