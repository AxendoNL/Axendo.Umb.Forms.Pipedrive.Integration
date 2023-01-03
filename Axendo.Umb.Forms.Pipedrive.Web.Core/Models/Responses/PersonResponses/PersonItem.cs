using Newtonsoft.Json;

namespace Axendo.Umb.Forms.Pipedrive.Web.Core.Models.Responses
{
    public class PersonItem
    {
        [JsonProperty("item")]
        public Person Person { get; set; }
    }
}
