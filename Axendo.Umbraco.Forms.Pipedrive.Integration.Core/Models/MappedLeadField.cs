using Newtonsoft.Json;

namespace Axendo.Umbraco.Forms.Pipedrive.Core.Models
{
    public class MappedLeadField
    {
        [JsonProperty(PropertyName = "formField")]
        public string FormField { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "leadField")]
        public string PipedriveLeadField { get; set; } = string.Empty;
    }
}

