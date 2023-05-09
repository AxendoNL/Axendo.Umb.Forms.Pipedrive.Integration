using Microsoft.Extensions.Configuration;
using System.Collections.Specialized;

namespace Axendo.Umbraco.Forms.Pipedrive.Core.Configuration
{
    public class PipedriveSettings
    {
        private readonly IConfiguration configuration;

        public PipedriveSettings(IConfiguration configuration) 
        {
            this.configuration = configuration;

            ApiKey = GetPipedriveSetting("ApiKey");
        }

        private string GetPipedriveSetting(string name)
        {
            return configuration?.GetSection("Pipedrive")[name] ?? string.Empty;
        }

        public string ApiKey { get; private set; }
    }
}
