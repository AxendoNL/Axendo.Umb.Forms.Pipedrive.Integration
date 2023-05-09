using Newtonsoft.Json;

namespace Axendo.Umbraco.Forms.Pipedrive.Core.Models
{
    public class MappedPersonField
    {
        [JsonProperty(PropertyName = "formField")]
        public string FormField { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "pipedriveField")]
        public string PipedriveField { get; set; } = string.Empty;
    }
}
