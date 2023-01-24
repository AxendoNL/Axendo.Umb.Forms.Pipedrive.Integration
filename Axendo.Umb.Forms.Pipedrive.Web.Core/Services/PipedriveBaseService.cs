using Axendo.Umb.Forms.Pipedrive.Web.Core.Configuration;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Umbraco.Core.Logging;

namespace Axendo.Umb.Forms.Pipedrive.Web.Core.Services
{
    public class PipedriveBaseService
    {
        private readonly PipedriveSettings Options;
        public readonly ILogger logger;
        private const string baseUrl = "https://api.pipedrive.com/v1/";

         public PipedriveBaseService(ILogger logger)
        {
            Options = new PipedriveSettings(ConfigurationManager.AppSettings);
            this.logger = logger;
        }
        
        public bool IsApiConfigurationValid()
        {
            if (string.IsNullOrEmpty(Options.ApiKey))
            {
                
                logger.Warn<PipedriveBaseService>("Api token cannot be null or empty");
                return false;
               
            }
           return true;
        }
        public string ApiUrl(string path)
        {
            return IsApiConfigurationValid() ? $"{baseUrl}{path}?api_token={Options.ApiKey}" : null;  
        }

        public string SetupApiUrlWithQueryParams(string path, Dictionary<string, string> queryParams)
        {
            if (IsApiConfigurationValid())
            {
                var queryString = queryParams != null && queryParams.Any() ? string.Join("&", queryParams.Select(q => $"{q.Key}={q.Value}")) : null;

                return $"{baseUrl}{path}?{queryString}&api_token={Options.ApiKey}";
            }

            return null;

        }
    }
}
