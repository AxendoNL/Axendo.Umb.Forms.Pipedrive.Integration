using Newtonsoft.Json;
using System.Collections.Generic;

namespace Axendo.Umbraco.Forms.Pipedrive.Core.Models.Responses
{
    public class PersonItemList
    {
        [JsonProperty("items")]
        public List<PersonItem> PersonItems { get; set; } = new();
    }
}
