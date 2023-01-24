using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
    public class PipedriveLeadService : PipedriveBaseService, ILeadService
    {
        private readonly static HttpClient _httpClient = new HttpClient();
        
        public PipedriveLeadService(ILogger logger) : base(logger) { }
       
        public async Task<IEnumerable<LeadField>> GetLeadFields()
        {
            var leadFields = new List<LeadField>();

            var url = ApiUrl("dealFields");
            var response = await _httpClient.GetAsync(new Uri(url));
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var personFieldsJson = JsonConvert.DeserializeObject<LeadFieldsResponse>(result);
                leadFields.AddRange(personFieldsJson.LeadFields);
            }
            else if (response.IsSuccessStatusCode == false)
            {
                logger.Error<PipedrivePersonService>("Failed to get LeadField for the url, {statusCode}", response.StatusCode);

                return Enumerable.Empty<LeadField>();
            }
            
            return leadFields.OrderBy(p => p.Name);
        }

        public async Task<PostResult> PostLead(Record record, List<MappedLeadField> mappedLeadFields, int personId)
        {
            PostResult leadResult = new PostResult() { Status = PipedriveStatus.Success };
            
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
                    logger.Warn<PipedrivePersonService>("The mapping form field did not match any recordField. Check if sensitive data is turned off.");
                    leadResult.Status =  PipedriveStatus.NotConfigured;

                    return leadResult;

                }
            }

            data.Add("person_id", personId);

            var url = ApiUrl("leads");
            var response = await _httpClient.PostAsJsonAsync(url, data).ConfigureAwait(false);


            if (response.IsSuccessStatusCode == false)
            {
                logger.Error<PipedrivePersonService>("Error submitting a Pipedrive person request. {Statuscode}, {Message}", response.StatusCode, response.ReasonPhrase);
                leadResult.Status = PipedriveStatus.Failed;

                return leadResult;
                    
            }

            return leadResult;
        }
    }
}
