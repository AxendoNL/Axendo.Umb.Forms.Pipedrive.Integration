using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Axendo.Umb.Forms.Pipedrive.Web.Core.Models.Responses
{
    public class PersonListRoot
    {
        [JsonProperty(PropertyName = "data")]
        public PersonItemList ItemList { get; set; }

    }
}
