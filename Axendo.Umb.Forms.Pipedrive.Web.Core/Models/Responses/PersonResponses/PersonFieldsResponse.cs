using Newtonsoft.Json;
using System.Collections.Generic;

namespace Axendo.Umb.Forms.Pipedrive.Web.Core.Models.Responses
{
    public class PersonFieldsResponse
    {
        [JsonProperty(PropertyName = "data")]
        public List<PersonField> Data { get; set; }
    }
}
