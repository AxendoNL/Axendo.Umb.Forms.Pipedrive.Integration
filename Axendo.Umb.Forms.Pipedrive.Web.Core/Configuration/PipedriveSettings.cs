using System.Collections.Specialized;

namespace Axendo.Umb.Forms.Pipedrive.Web.Core.Configuration
{
    public class PipedriveSettings
    {
        public PipedriveSettings() { }

        public PipedriveSettings(NameValueCollection appSettings)
        {
            ApiKey = appSettings["PipedriveApiKey"];
        }

        public string ApiKey { get; set; }
    }
}
