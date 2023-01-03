using Newtonsoft.Json;
using System.Collections.Generic;

namespace Axendo.Umb.Forms.Pipedrive.Web.Core.Models.Responses.LeadResponses
{
    public class LeadFieldsResponse
    {
        [JsonProperty(PropertyName = "data")]
        public List<LeadField> LeadFields { get; set; }
    }
}
