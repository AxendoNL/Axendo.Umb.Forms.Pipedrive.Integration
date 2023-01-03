using Newtonsoft.Json;
using System.Collections.Generic;

namespace Axendo.Umb.Forms.Pipedrive.Web.Core.Models.Responses
{
    public class PersonItemList
    {
        [JsonProperty("items")]
        public List<PersonItem> PersonItems { get; set; }
    }
}
