using Axendo.Umbraco.Forms.Pipedrive.Core.Services;
using Axendo.Umbraco.Forms.Pipedrive.Core.Models.Responses;
using Axendo.Umbraco.Forms.Pipedrive.Core.Models.Responses.LeadResponses;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;

namespace Axendo.Umbraco.Forms.Pipedrive.Core.Controller
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
