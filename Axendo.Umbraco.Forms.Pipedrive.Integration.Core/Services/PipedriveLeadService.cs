using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Axendo.Umbraco.Forms.Pipedrive.Core.Models;
using Axendo.Umbraco.Forms.Pipedrive.Core.Models.Responses;
using Axendo.Umbraco.Forms.Pipedrive.Core.Models.Responses.LeadResponses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Forms.Core.Persistence.Dtos;

namespace Axendo.Umbraco.Forms.Pipedrive.Core.Services
{
    public class PipedriveLeadService : PipedriveBaseService, ILeadService
    {
        private readonly static HttpClient _httpClient = new HttpClient();

        public PipedriveLeadService(IConfiguration configuration, ILogger<PipedrivePersonService> logger)
            : base(configuration, logger)
        {
        }

        public async Task<IEnumerable<LeadField>> GetLeadFields()
        {
            List<LeadField> leadFields = new();

            string url = ApiUrl("dealFields");
            HttpResponseMessage response = await _httpClient.GetAsync(new Uri(url));
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                LeadFieldsResponse? personFieldsJson = JsonConvert.DeserializeObject<LeadFieldsResponse>(result);
                if (personFieldsJson is not null)
                {
                    leadFields.AddRange(personFieldsJson.LeadFields);
                }
            }
            else if (response.IsSuccessStatusCode == false)
            {
                logger.LogError("Failed to get LeadField for the url, {statusCode}", response.StatusCode);

                return Enumerable.Empty<LeadField>();
            }

            return leadFields.OrderBy(p => p.Name);
        }

        public async Task<PostResult> PostLead(Record record, List<MappedLeadField> mappedLeadFields, int personId)
        {
            PostResult leadResult = new PostResult() { Status = PipedriveStatus.Success };

            JObject data = new();

            foreach (MappedLeadField mapping in mappedLeadFields)
            {
                RecordField? formRecordField = record.GetRecordField(new Guid(mapping.FormField));
                if (formRecordField is not null)
                {
                    data.Add(mapping.PipedriveLeadField, formRecordField.ValuesAsString());
                }
                else
                {
                    logger.LogWarning("The mapping form field did not match any recordField. Check if sensitive data is turned off.");
                    leadResult.Status = PipedriveStatus.NotConfigured;

                    return leadResult;
                }
            }

            data.Add("person_id", personId);

            string url = ApiUrl("leads");
            HttpResponseMessage? response = await _httpClient.PostAsJsonAsync(url, data).ConfigureAwait(false);

            if (response is null)
            {
                logger.LogError("Error submitting a Pipedrive person request. {Statuscode}, {Message}", "-", "<unknown>");
                leadResult.Status = PipedriveStatus.Failed;

                return leadResult;
            }
            else if (response.IsSuccessStatusCode == false)
            {
                logger.LogError("Error submitting a Pipedrive person request. {Statuscode}, {Message}", response.StatusCode, response.ReasonPhrase);
                leadResult.Status = PipedriveStatus.Failed;

                return leadResult;
            }

            return leadResult;
        }
    }
}
