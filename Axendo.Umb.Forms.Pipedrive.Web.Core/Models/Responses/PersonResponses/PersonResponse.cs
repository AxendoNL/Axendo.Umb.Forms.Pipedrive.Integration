using Newtonsoft.Json;


namespace Axendo.Umb.Forms.Pipedrive.Web.Core.Models.Responses
{
    public class PersonResponse
    {
        [JsonProperty(PropertyName = "data")]
        public Person Person { get; set; }

        
    }
}
