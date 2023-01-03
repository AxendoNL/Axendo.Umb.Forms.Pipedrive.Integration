using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Axendo.Umb.Forms.Pipedrive.Web.Core.Models.Responses.LeadResponses;
using Axendo.Umb.Forms.Pipedrive.Web.Core.Models.Responses;
using Axendo.Umb.Forms.Pipedrive.Web.Core.Services;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;


namespace Axendo.Umb.Forms.Pipedrive.Web.Core.Controller
{
    [PluginController("UmbracoFormsIntegrationsCrmPipedrive")]
    
    public class PipedriveController : UmbracoAuthorizedJsonController
    {
        private readonly IPersonService _personService;
        private readonly ILeadService _leadService;

        public PipedriveController(IPersonService personService, ILeadService leadService)
        {
            _personService = personService;
            _leadService = leadService;
        }

        [HttpGet]
        public async Task<IEnumerable<PersonField>> GetAllPersonFields()
        {
            return await _personService.GetPersonFields();
        }

        [HttpGet]
        public async Task<IEnumerable<LeadField>> GetAllLeadFields()
        {
            return await _leadService.GetLeadFields();
        }
    }
}
