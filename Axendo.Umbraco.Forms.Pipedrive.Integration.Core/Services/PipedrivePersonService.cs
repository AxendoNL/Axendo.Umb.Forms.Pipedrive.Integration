using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Axendo.Umbraco.Forms.Pipedrive.Core.Models;
using Axendo.Umbraco.Forms.Pipedrive.Core.Models.Responses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Forms.Core.Persistence.Dtos;

namespace Axendo.Umbraco.Forms.Pipedrive.Core.Services
{
    public class PipedrivePersonService : PipedriveBaseService, IPersonService
    {
        private readonly static HttpClient _httpClient = new();

        public PipedrivePersonService(IConfiguration configuration, ILogger<PipedrivePersonService> logger)
            : base(configuration, logger)
        {
        }

        public async Task<IEnumerable<PersonField>> GetPersonFields()
        {
            List<PersonField> personFields = new();

            string url = ApiUrl("personFields");
            HttpResponseMessage response = await _httpClient.GetAsync(new Uri(url));
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                PersonFieldsResponse? personFieldsJson = JsonConvert.DeserializeObject<PersonFieldsResponse>(result);
                if (personFieldsJson is not null)
                {
                    personFields.AddRange(personFieldsJson.Data);
                }
            }
            else if (response.IsSuccessStatusCode == false)
            {
                logger.LogError("Failed to get Personfields for the url, {statusCode}", response.StatusCode);

                return Enumerable.Empty<PersonField>();
            }

            return personFields.OrderBy(p => p.Name);
        }

        public async Task<PostResult<Person>> PostPerson(Record record, List<MappedPersonField> mappedPersonFields)
        {
            PostResult<Person> personResult = new PostResult<Person>() { Status = PipedriveStatus.Success };

            JObject data = new();

            foreach (MappedPersonField mapping in mappedPersonFields)
            {
                RecordField? formRecordField = record.GetRecordField(new Guid(mapping.FormField));
                if (formRecordField is not null)
                {
                    data.Add(mapping.PipedriveField, formRecordField.ValuesAsString());
                }
                else
                {
                    logger.LogWarning("The mapping form field did not match any recordField. Check if sensitive data is turned off.");
                    personResult.Status = PipedriveStatus.NotConfigured;

                    return personResult;

                }
            }

            string? email = data.GetValue("email")?.ToString();

            IEnumerable<PersonItem> personItems = await SearchPersonsByEmail(email).ConfigureAwait(false);
            IEnumerable<PersonItem> existingPersons = personItems.Where(p => p.Person.Email == email);

            if (!existingPersons.Any())
            {
                string? url = ApiUrl("persons");
                HttpResponseMessage? response = await _httpClient.PostAsJsonAsync(url, data).ConfigureAwait(false);

                if (response is null)
                {
                    logger.LogError("Error submitting a Pipedrive person request. {Message}", "<unknown>");
                    personResult.Status = PipedriveStatus.Failed;

                    return personResult;
                }

                string? result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode == false)
                {
                    logger.LogError("Error submitting a Pipedrive person request. {Message}", $"{response.ReasonPhrase} - {result}");
                    personResult.Status = PipedriveStatus.Failed;

                    return personResult;
                }

                PersonResponse? personJson = JsonConvert.DeserializeObject<PersonResponse>(result);
                if (personJson is not null)
                {
                    personResult.Result = personJson.Person;
                }
            }
            else
            {
                IEnumerable<PersonItem> personItemList = await SearchPersonsByEmail(email);
                Person person = personItemList.First().Person;

                personResult.Result = person;
            }

            return personResult;
        }

        public async Task<IEnumerable<PersonItem>> SearchPersonsByEmail(string? email)
        {
            if (email == null)
            {
                logger.LogError("Failed to get persons for the email, {statusCode}", "-");

                return Enumerable.Empty<PersonItem>();
            }

            List<PersonItem> persons = new();

            Dictionary<string, string> queryParams = new()
                {
                    {"term", email },
                    {"fields","email" },
                    {"exact_match", "true"}
                };

            string? url = SetupApiUrlWithQueryParams("persons/search", queryParams);
            HttpResponseMessage? response = await _httpClient.GetAsync(url).ConfigureAwait(false);

            if (response is null)
            {
                logger.LogError("Failed to get persons for the url, {statusCode}", "-");

                return Enumerable.Empty<PersonItem>();
            }
            else if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                PersonListRoot? personJson = JsonConvert.DeserializeObject<PersonListRoot>(result);
                if (personJson is not null)
                {
                    persons.AddRange(personJson.ItemList.PersonItems);
                }
            }
            else if (response.IsSuccessStatusCode == false)
            {
                logger.LogError("Failed to get persons for the url, {statusCode}", response.StatusCode);

                return Enumerable.Empty<PersonItem>();
            }

            return persons;
        }
    }
}

