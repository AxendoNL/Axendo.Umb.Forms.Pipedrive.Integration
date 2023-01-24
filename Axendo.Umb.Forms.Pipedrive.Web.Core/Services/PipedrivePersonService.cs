using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Umbraco.Core.Logging;
using System.Threading.Tasks;
using Umbraco.Forms.Core.Persistence.Dtos;
using Axendo.Umb.Forms.Pipedrive.Web.Core.Models;
using Axendo.Umb.Forms.Pipedrive.Web.Core.Models.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Axendo.Umb.Forms.Pipedrive.Web.Core.Services
{
    public class PipedrivePersonService : PipedriveBaseService, IPersonService
    {
        private readonly static HttpClient _httpClient = new HttpClient();
        
        public PipedrivePersonService(ILogger logger) : base(logger) { }
        
        public async Task<IEnumerable<PersonField>> GetPersonFields()
        {
            var personFields = new List<PersonField>();

            var url = ApiUrl("personFields");
            var response = await _httpClient.GetAsync(new Uri(url));
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var personFieldsJson = JsonConvert.DeserializeObject<PersonFieldsResponse>(result);
                personFields.AddRange(personFieldsJson.Data);
            }
            else if (response.IsSuccessStatusCode == false)
            {
                logger.Error<PipedrivePersonService>("Failed to get Personfields for the url, {statusCode}", response.StatusCode);

                return Enumerable.Empty<PersonField>();
            }
            
            return personFields.OrderBy(p => p.Name);
        }

        public async Task<PostResult<Person>> PostPerson(Record record, List<MappedPersonField> mappedPersonFields)
        {
            PostResult<Person> personResult = new PostResult<Person>() { Status = PipedriveStatus.Success};

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
                    logger.Warn<PipedrivePersonService>("The mapping form field did not match any recordField. Check if sensitive data is turned off.");
                    personResult.Status = PipedriveStatus.NotConfigured;
                        
                    return personResult;

                }
            }

            var email = data.GetValue("email").ToString();
            
            
                
            var personItems = await SearchPersonsByEmail(email).ConfigureAwait(false);
            var existingPersons = personItems.Where(p => p.Person.Email== email);
                
            if (!existingPersons.Any())
            {
                var url = ApiUrl("persons");
                var response = await _httpClient.PostAsJsonAsync(url, data).ConfigureAwait(false);


                if (response.IsSuccessStatusCode == false)
                {
                    logger.Error<PipedrivePersonService>("Error submitting a Pipedrive person request. {Message}", response.ReasonPhrase);
                    personResult.Status = PipedriveStatus.Failed;

                    return personResult;

                }

                var result = await response.Content.ReadAsStringAsync();
                var personJson = JsonConvert.DeserializeObject<PersonResponse>(result);
                personResult.Result = personJson.Person;
            }
            else
            {
                var personItemList = await SearchPersonsByEmail(email);
                var person = personItemList.First().Person;

                personResult.Result = person;
            }
            
            return personResult;
        }
        public async Task<IEnumerable<PersonItem>> SearchPersonsByEmail(string email)
        {
            var persons = new List<PersonItem>();

            Dictionary<string, string> queryParams = new Dictionary<string, string>()
            { 
                {"term", email },
                {"fields","email" },
                {"exact_match", "true"}
            };
            var url = SetupApiUrlWithQueryParams("persons/search", queryParams);
            var response = await _httpClient.GetAsync(url).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var personJson = JsonConvert.DeserializeObject<PersonListRoot>(result);
                persons.AddRange(personJson.ItemList.PersonItems);
            }
            else if (response.IsSuccessStatusCode == false)
            {
                logger.Error<PipedrivePersonService>("Failed to get persons for the url, {statusCode}", response.StatusCode);

                return Enumerable.Empty<PersonItem>();
            }
            
           return persons;
        }
    }
}

