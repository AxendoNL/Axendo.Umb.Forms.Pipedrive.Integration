using Newtonsoft.Json;

namespace Axendo.Umb.Forms.Pipedrive.Web.Core.Models
{
    public class MappedLeadField
    {
        [JsonProperty(PropertyName = "formField")]
        public string FormField { get; set; }

        [JsonProperty(PropertyName = "leadField")]
        public string PipedriveLeadField { get; set; }
    }
}

