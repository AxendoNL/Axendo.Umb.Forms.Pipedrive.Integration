using Newtonsoft.Json;
using System.Collections.Generic;

namespace Axendo.Umbraco.Forms.Pipedrive.Core.Models.Responses
{
    public class PersonFieldsResponse
    {
        [JsonProperty(PropertyName = "data")]
        public List<PersonField> Data { get; set; } = new();
    }
}
