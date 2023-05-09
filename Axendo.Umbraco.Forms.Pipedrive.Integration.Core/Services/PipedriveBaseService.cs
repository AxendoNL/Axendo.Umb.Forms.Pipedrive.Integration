using Axendo.Umbraco.Forms.Pipedrive.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Axendo.Umbraco.Forms.Pipedrive.Core.Services
{
    public class PipedriveBaseService
    {
        private readonly PipedriveSettings Options;
        public readonly ILogger<PipedriveBaseService> logger;

        private const string baseUrl = "https://api.pipedrive.com/v1/";

        public PipedriveBaseService(IConfiguration configuration, ILogger<PipedriveBaseService> logger)
        {
            Options = new(configuration);
            this.logger = logger;
        }

        public bool IsApiConfigurationValid()
        {
            if (string.IsNullOrEmpty(Options.ApiKey))
            {
                logger.LogWarning("Api token cannot be null or empty");
                return false;

            }
            return true;
        }
        public string ApiUrl(string path)
        {
            return IsApiConfigurationValid() ? $"{baseUrl}{path}?api_token={Options.ApiKey}" : string.Empty;
        }

        public string SetupApiUrlWithQueryParams(string path, Dictionary<string, string> queryParams)
        {
            if (IsApiConfigurationValid())
            {
                string queryString = queryParams != null && queryParams.Any() ? string.Join("&", queryParams.Select(q => $"{q.Key}={q.Value}")) : string.Empty;

                return $"{baseUrl}{path}?{queryString}&api_token={Options.ApiKey}";
            }

            return string.Empty;
        }
    }
}
