using Axendo.Umbraco.Forms.Pipedrive.Core.Models;
using Axendo.Umbraco.Forms.Pipedrive.Core.Models.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;
using Umbraco.Forms.Core.Persistence.Dtos;

namespace Axendo.Umbraco.Forms.Pipedrive.Core.Services
{
    public interface IPersonService
    {
        Task<IEnumerable<PersonField>> GetPersonFields();

        Task<PostResult<Person>> PostPerson(Record record, List<MappedPersonField> mappedPersonFields);
    }
}
