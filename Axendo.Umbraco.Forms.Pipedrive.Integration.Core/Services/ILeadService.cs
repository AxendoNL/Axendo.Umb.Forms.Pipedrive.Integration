using Axendo.Umbraco.Forms.Pipedrive.Core.Models;
using Axendo.Umbraco.Forms.Pipedrive.Core.Models.Responses;
using Axendo.Umbraco.Forms.Pipedrive.Core.Models.Responses.LeadResponses;
using System.Collections.Generic;
using System.Threading.Tasks;
using Umbraco.Forms.Core.Persistence.Dtos;

namespace Axendo.Umbraco.Forms.Pipedrive.Core.Services
{
    public interface ILeadService
    {
        Task<IEnumerable<LeadField>> GetLeadFields();

        Task<PostResult> PostLead(Record record, List<MappedLeadField> mappedLeadFields, int personId);
    }
}

