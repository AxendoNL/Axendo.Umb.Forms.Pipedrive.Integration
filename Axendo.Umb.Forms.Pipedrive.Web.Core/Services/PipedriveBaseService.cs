using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Logging;
using Umbraco.Forms.Core;

namespace Axendo.Umb.Forms.Pipedrive.Web.Core.Services
{
    public class PipedriveBaseService : IPipedriveBaseService
    {
        private readonly IFacadeConfiguration _configuration;
        private readonly ILogger _logger;

        private const string baseUrl = "https://api.pipedrive.com/v1/";

        public PipedriveBaseService(IFacadeConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        private string _apiKey { get; set; }

        public bool GetApiKey(out string apiKey)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                apiKey = _configuration.GetSetting("PipedriveApiKey");
                if (string.IsNullOrEmpty(apiKey))
                {
                    _logger.Warn<PipedrivePersonService>("Api token cannot be null or empty");
                    return false;
                }
            }
            else
            {
                apiKey = _apiKey;
                
            }

            return true;
        }
        public string ApiUrl(string path, string apiKey)
        {

            return $"{baseUrl}{path}?api_token={apiKey}";
        }

        public string SetupApiUrlWithQueryParams(string path, Dictionary<string, string> queryParams, string apiKey)
        {
            var queryString = queryParams != null && queryParams.Any() ? string.Join("&", queryParams.Select(q => $"{q.Key}={q.Value}")) : null;

            return $"{baseUrl}{path}?{queryString}&api_token={apiKey}";

        }
    }
}
