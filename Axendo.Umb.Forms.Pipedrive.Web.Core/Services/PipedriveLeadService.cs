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
using Axendo.Umb.Forms.Pipedrive.Web.Core.Models.Responses;

namespace Axendo.Umb.Forms.Pipedrive.Web.Core.Services
{
    public class PipedriveLeadService : ILeadService
    {
        private readonly static HttpClient _httpClient = new HttpClient();
        private readonly IPipedriveBaseService _pipedriveBaseService;
        private readonly ILogger _logger;

        public PipedriveLeadService(IPipedriveBaseService pipedriveBaseService,ILogger logger)
        {
            _pipedriveBaseService = pipedriveBaseService;
            _logger = logger;
        }
        public async Task<IEnumerable<LeadField>> GetLeadFields()
        {
            var leadFields = new List<LeadField>();

            if (_pipedriveBaseService.GetApiKey(out string apiKey))
            {
                var url = _pipedriveBaseService.ApiUrl("dealFields", apiKey);
                var response = await _httpClient.GetAsync(new Uri(url));
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

        public async Task<PostResult> PostLead(Record record, List<MappedLeadField> mappedLeadFields, int personId)
        {
            PostResult leadResult = new PostResult() { Status = PipedriveStatus.Success };
            
            if (_pipedriveBaseService.GetApiKey(out string apiKey))
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
                        leadResult.Status =  PipedriveStatus.NotConfigured;

                        return leadResult;

                    }
                }

                data.Add("person_id", personId);

                var url = _pipedriveBaseService.ApiUrl("leads", apiKey);
                var response = await _httpClient.PostAsJsonAsync(url, data).ConfigureAwait(false);


                if (response.IsSuccessStatusCode == false)
                {
                    _logger.Error<PipedrivePersonService>("Error submitting a Pipedrive person request. {Statuscode}, {Message}", response.StatusCode, response.ReasonPhrase);
                    leadResult.Status = PipedriveStatus.Failed;

                    return leadResult;
                    
                }

            }
            return leadResult;
        }
    }
}
