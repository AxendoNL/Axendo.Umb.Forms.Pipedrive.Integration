using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Umbraco.Forms.Core;
using Umbraco.Core.Logging;
using System.Threading.Tasks;
using Umbraco.Forms.Core.Persistence.Dtos;
using Axendo.Umb.Forms.Pipedrive.Web.Core.Models;
using Axendo.Umb.Forms.Pipedrive.Web.Core.Models.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Axendo.Umb.Forms.Pipedrive.Web.Core.Services
{
    public class PipedrivePersonService : IPersonService
    {
        private readonly static HttpClient _httpClient = new HttpClient();
        private readonly IFacadeConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly PipedriveLeadService _pipedriveLeadService;

        private const string baseUrl = "https://api.pipedrive.com/v1/";


        public PipedrivePersonService(IFacadeConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
            _pipedriveLeadService = new PipedriveLeadService(configuration, logger);
            
        }

        public async Task<IEnumerable<PersonField>> GetPersonFields()
        {
            var personFields = new List<PersonField>();

            if (GetApiKey(out string apiKey))
            {
                var response = await _httpClient.GetAsync(new Uri(ApiUrl("personFields", apiKey)));
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var personFieldsJson = JsonConvert.DeserializeObject<PersonFieldsResponse>(result);
                    personFields.AddRange(personFieldsJson.Data);
                }
                else if (response.IsSuccessStatusCode == false)
                {
                    _logger.Error<PipedrivePersonService>("Failed to get Personfields for the url, {statusCode}", response.StatusCode);

                    return Enumerable.Empty<PersonField>();
                }
            }
            else
            {
                _logger.Error<PipedrivePersonService>("Failed to get PersonFields.There is no Api key configured");

                return Enumerable.Empty<PersonField>();
            }

            return personFields.OrderBy(p => p.Name);
        }

        public async Task<WorkflowStatus> PostPersonAndLead(Record record, List<MappedPersonField> mappedPersonFields, List<MappedLeadField> mappedLeadFields)
        {

            if (GetApiKey(out string apiKey))
            {

                var data = new JObject();

                foreach (var mapping in mappedPersonFields)
                {
                    var formRecordField = record.GetRecordField(new Guid(mapping.FormField));
                    if (formRecordField != null)
                    {
                        data.Add(mapping.PipedriveField, formRecordField.ValuesAsString());
                    }
                    else
                    {
                        _logger.Warn<PipedrivePersonService>("The mapping form field did not match any recordField. Check if sensitive data is turned off.");
                        return WorkflowStatus.NotConfigured;

                    }
                }

                var email = data.GetValue("email").ToString();

                if (await CheckIfEmailExists(email).ConfigureAwait(false))
                {
                    var url = ApiUrl("persons", apiKey);
                    var response = await _httpClient.PostAsJsonAsync(url, data).ConfigureAwait(false);


                    if (response.IsSuccessStatusCode == false)
                    {
                        _logger.Error<PipedrivePersonService>("Error submitting a Pipedrive person request ");

                        return WorkflowStatus.Failed;
                    }

                    var result = await response.Content.ReadAsStringAsync();
                    var personJson = JsonConvert.DeserializeObject<PersonResponse>(result);
                    var person = personJson.Person;

                    if (person != null)
                    {
                        return await _pipedriveLeadService.PostLead(record, mappedLeadFields, person.Id);
                    }
                    else
                    {
                        _logger.Warn<PipedrivePersonService>("No person found with the given Id!");
                        return WorkflowStatus.Failed;
                    }

                }
                else
                {
                    var personItemList = await SearchPersonsByEmail(email);

                    
                    foreach(var personItem in personItemList)
                    {
                        var personId = personItem.Person.Id;
                        return await _pipedriveLeadService.PostLead(record, mappedLeadFields, personId);
                    }
                }

             }

            return WorkflowStatus.Success;
        }

        public async Task<bool> CheckIfEmailExists(string email)
        {

            bool result = true;
            var persons = await SearchPersonsByEmail(email).ConfigureAwait(false);


            foreach (var person in persons)
            {
                if (email == person.Person.Email)
                {
                    
                    result =  false;
                }
            }
            return result;
        }

        public async Task<IEnumerable<PersonItem>> SearchPersonsByEmail(string email)
        {
            var persons = new List<PersonItem>();

            if (GetApiKey(out string apiKey))
            {
                Dictionary<string, string> queryParams = new Dictionary<string, string>()
                { 
                    {"term", email },
                    {"fields","email" },
                    {"exact_match", "true"}
                };
                var url = SetupApiUrlWithQueryParams("persons/search", queryParams, apiKey);
                var response = await _httpClient.GetAsync(url).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var personJson = JsonConvert.DeserializeObject<PersonListRoot>(result);
                    persons.AddRange(personJson.ItemList.PersonItems);
                }
                else if (response.IsSuccessStatusCode == false)
                {
                    _logger.Error<PipedrivePersonService>("Failed to get persons for the url, {statusCode}", response.StatusCode);

                    return Enumerable.Empty<PersonItem>();
                }
            }
            else
            {
                _logger.Error<PipedrivePersonService>("Failed to get persons.There is no Api key configured");

                return Enumerable.Empty<PersonItem>();
            }

            return persons;

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
            
            return $"{baseUrl}{path}?api_token={apiKey}";
        }

        private string SetupApiUrlWithQueryParams(string path, Dictionary<string, string> queryParams, string apiKey)
        {
            var queryString = queryParams != null && queryParams.Any() ? string.Join("&", queryParams.Select(q => $"{q.Key}={q.Value}")) : null;

            return $"{baseUrl}{path}?{queryString}&api_token={apiKey}";

        }
    }
}

