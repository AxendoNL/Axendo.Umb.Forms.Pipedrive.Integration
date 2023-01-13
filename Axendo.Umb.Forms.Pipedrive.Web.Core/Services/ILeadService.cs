using System.Collections.Generic;
using System.Threading.Tasks;
using Umbraco.Forms.Core.Persistence.Dtos;
using Axendo.Umb.Forms.Pipedrive.Web.Core.Models;
using Axendo.Umb.Forms.Pipedrive.Web.Core.Models.Responses.LeadResponses;
using Axendo.Umb.Forms.Pipedrive.Web.Core.Models.Responses;

namespace Axendo.Umb.Forms.Pipedrive.Web.Core.Services
{
    public interface ILeadService
    {
        Task<IEnumerable<LeadField>> GetLeadFields();

        Task<PostResult> PostLead(Record record, List<MappedLeadField> mappedLeadFields, int personId);
    }
}

