using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Umbraco.Forms.Core;
using Umbraco.Core.Logging;
using System.Threading.Tasks;
using Umbraco.Forms.Core.Persistence.Dtos;
using Axendo.Umb.Forms.Pipedrive.Web.Core.Models;
using Axendo.Umb.Forms.Pipedrive.Web.Core.Models.Responses.LeadResponses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Axendo.Umb.Forms.Pipedrive.Web.Core.Services
{
    public class PipedriveLeadService : ILeadService
    {
        private readonly static HttpClient _httpClient = new HttpClient();
        private readonly IFacadeConfiguration _configuration;
        private readonly ILogger _logger;

        public PipedriveLeadService(IFacadeConfiguration facadeConfiguration, ILogger logger)
        {
            _configuration = facadeConfiguration;
            _logger = logger;
        }
        public async Task<IEnumerable<LeadField>> GetLeadFields()
        {
            var leadFields = new List<LeadField>();

            if (GetApiKey(out string apiKey))
            {
                var response = await _httpClient.GetAsync(new Uri(ApiUrl("dealFields", apiKey)));
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var personFieldsJson = JsonConvert.DeserializeObject<LeadFieldsResponse>(result);
                    leadFields.AddRange(personFieldsJson.LeadFields);
                }
                else if (response.IsSuccessStatusCode == false)
                {
                    _logger.Error<PipedrivePersonService>("Failed to get LeadField for the url, {statusCode}", response.StatusCode);

                    return Enumerable.Empty<LeadField>();
                }
            }
            else
            {
                _logger.Error<PipedrivePersonService>("Failed to get Leadfields.There is no Api key configured");

                return Enumerable.Empty<LeadField>();
            }

            return leadFields.OrderBy(p => p.Name);
        }

        public async Task<WorkflowStatus> PostLead(Record record, List<MappedLeadField> mappedLeadFields, int personId)
        {
            if (GetApiKey(out string apiKey))
            {

                var data = new JObject();

                foreach (var mapping in mappedLeadFields)
                {
                    var formRecordField = record.GetRecordField(new Guid(mapping.FormField));
                    if (formRecordField != null)
                    {
                        data.Add(mapping.PipedriveLeadField, formRecordField.ValuesAsString());
                    }
                    else
                    {
                        _logger.Warn<PipedrivePersonService>("The mapping form field did not match any recordField. Check if sensitive data is turned off.");
                        return WorkflowStatus.NotConfigured;

                    }
                }

                data.Add("person_id", personId);

                var url = ApiUrl("leads", apiKey);
                var response = await _httpClient.PostAsJsonAsync(url, data).ConfigureAwait(false);


                if (response.IsSuccessStatusCode == false)
                {
                    _logger.Error<PipedrivePersonService>("Error submitting a Pipedrive person request ");

                    return WorkflowStatus.Failed;
                }

            }

            return WorkflowStatus.Success;

            }

        private bool GetApiKey(out string apiKey)
        {
            apiKey = _configuration.GetSetting("PipedriveApiKey");
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.Warn<PipedrivePersonService>("Api token cannot be null or empty");
            }
            return true;

        }
        private string ApiUrl(string path, string apiKey)
        {
            const string baseUrl = "https://api.pipedrive.com/v1/";


            return $"{baseUrl}{path}?api_token={apiKey}";
        }
    }
}
