using System.Collections.Generic;
using System.Threading.Tasks;
using Umbraco.Forms.Core.Persistence.Dtos;
using Axendo.Umb.Forms.Pipedrive.Web.Core.Models;
using Axendo.Umb.Forms.Pipedrive.Web.Core.Models.Responses;

namespace Axendo.Umb.Forms.Pipedrive.Web.Core.Services
{
   public interface IPersonService
    {
        Task<IEnumerable<PersonField>> GetPersonFields();

        Task<WorkflowStatus> PostPersonAndLead(Record record, List<MappedPersonField> mappedPersonFields, List<MappedLeadField> mappedLeadFields);
    }
}
