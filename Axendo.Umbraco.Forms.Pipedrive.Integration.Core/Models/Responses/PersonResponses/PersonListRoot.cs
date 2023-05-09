using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Axendo.Umbraco.Forms.Pipedrive.Core.Models.Responses
{
    public class PersonListRoot
    {
        [JsonProperty(PropertyName = "data")]
        public PersonItemList ItemList { get; set; } = new();
    }
}
