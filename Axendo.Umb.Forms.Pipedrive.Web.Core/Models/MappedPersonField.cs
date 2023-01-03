using Newtonsoft.Json;

namespace Axendo.Umb.Forms.Pipedrive.Web.Core.Models
{
    public class MappedPersonField
    {
        [JsonProperty(PropertyName = "formField")]
        public string FormField { get; set; }

        [JsonProperty(PropertyName = "pipedriveField")]
        public string PipedriveField { get; set; }
    }
}
