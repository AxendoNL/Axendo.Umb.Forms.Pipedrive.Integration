using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Umbraco.Core.Logging;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Persistence.Dtos;
using Axendo.Umb.Forms.Pipedrive.Web.Core.Services;
using Axendo.Umb.Forms.Pipedrive.Web.Core.Models;


namespace Axendo.Umb.Forms.Pipedrive.Web.Core.Workflow
{
    public class PipedriveWorkflow : WorkflowType
    {
        private readonly ILogger _logger;
        private readonly IPersonService _personService;

        public PipedriveWorkflow(ILogger logger, IPersonService personService)
        {
            _logger = logger;
            _personService = personService;

            Id = new Guid("ff92cdf3-322e-48be-a505-11076662273a");
            Name = "Save Person to Pipedrive";
            Description = "Submission of a form will be sent to Pipedrive CRM";
            Icon = "icon-handshake";
            Group = "CRM";
        }

        [Setting("personField Mappings",
            Description = "Map form fields to Pipedrive PersonFields",
            View = "~/App_Plugins/UmbracoForms.integrations/Crm/Pipedrive/Person/pipedrive.personfields.html")]
        public string PersonFieldMappings { get; set; }

        [Setting("leadField Mappings",
            Description = "Map form fields to Pipedrive PersonFields",
            View = "~/App_Plugins/UmbracoForms.integrations/Crm/Pipedrive/Lead/pipedrive.leadfields.html")]
        public string LeadFieldMappings { get; set; }


            public override WorkflowExecutionStatus Execute(Record record, RecordEventArgs e)
        {
            List<MappedPersonField> mappedPersonFields = JsonConvert.DeserializeObject<List<MappedPersonField>>(PersonFieldMappings);
            
            List<MappedLeadField> mappedLeadFields = JsonConvert.DeserializeObject<List<MappedLeadField>>(LeadFieldMappings);

            if (mappedPersonFields.Count > 0 && mappedLeadFields.Count > 0)
            {
                WorkflowStatus status = _personService.PostPersonAndLead(record, mappedPersonFields,mappedLeadFields).GetAwaiter().GetResult();

                if (status == WorkflowStatus.Success)
                {
                    return WorkflowExecutionStatus.Completed;
                }
                else if (status == WorkflowStatus.Failed)
                {
                    _logger.Warn<PipedriveWorkflow>("Failed to execute the workflow {WorkflowName} for {FormName} ({FormId})",
                                                     Workflow.Name, e.Form.Name, e.Form.Id);
                    return WorkflowExecutionStatus.Failed;
                }
                else if (status == WorkflowStatus.NotConfigured)
                {
                    _logger.Warn<PipedriveWorkflow>("Could not execute the workflow {WorkflowName} for {FormName} ({FormId}), because workflow is not correctly configured",
                                                     Workflow.Name, e.Form.Name, e.Form.Id);
                    return WorkflowExecutionStatus.NotConfigured;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(status));
                }
            }
            else
            {
                _logger.Warn<PipedriveWorkflow>("Missing Pipedrive field mapping and/or product title for the workflow {WorkflowName} for the form {FormName} ({FormId})",
                                                 Workflow.Name, e.Form.Name, e.Form.Id);
                return WorkflowExecutionStatus.NotConfigured;
            }
        }

        public override List<Exception> ValidateSettings()
        {
            var errors = new List<Exception>();
            return errors;
        }
    }
}
