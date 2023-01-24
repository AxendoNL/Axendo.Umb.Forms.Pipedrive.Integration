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
using System.Linq;
using Axendo.Umb.Forms.Pipedrive.Web.Core.Models.Responses;


namespace Axendo.Umb.Forms.Pipedrive.Web.Core.Workflow
{
    public class PipedriveWorkflow : WorkflowType
    {
        private readonly ILogger _logger;
        private readonly IPersonService _personService;
        private readonly ILeadService _leadService;
        public PipedriveWorkflow(ILogger logger, IPersonService personService, ILeadService leadService)
        {
            _logger = logger;
            _personService = personService;
            _leadService = leadService;
            

            Id = new Guid("ff92cdf3-322e-48be-a505-11076662273a");
            Name = "Save Person to Pipedrive";
            Description = "Submission of a form will be sent to Pipedrive CRM";
            Icon = "icon-handshake";
            Group = "CRM";
            
        }

        [@Setting("personField Mappings",
            Description = "Map form fields to Pipedrive PersonFields",
            View = "~/App_Plugins/UmbracoForms.integrations/Crm/Pipedrive/Person/pipedrive.personfields.html")]
        public string PersonFieldMappings { get; set; }

        [@Setting("leadField Mappings",
            Description = "Map form fields to Pipedrive PersonFields",
            View = "~/App_Plugins/UmbracoForms.integrations/Crm/Pipedrive/Lead/pipedrive.leadfields.html")]
        public string LeadFieldMappings { get; set; }


            public override WorkflowExecutionStatus Execute(Record record, RecordEventArgs e)
            {
            List<MappedPersonField> mappedPersonFields = JsonConvert.DeserializeObject<List<MappedPersonField>>(PersonFieldMappings);
            
            List<MappedLeadField> mappedLeadFields = JsonConvert.DeserializeObject<List<MappedLeadField>>(LeadFieldMappings);

            PostResult executionStatus = new PostResult() { Status = PipedriveStatus.Unknown};

            if (mappedPersonFields.Count > 0 && mappedLeadFields.Count > 0)
            {
                PostResult<Person> person = _personService.PostPerson(record, mappedPersonFields).GetAwaiter().GetResult();
                executionStatus.Status = person.Status;
                if (executionStatus.Status == PipedriveStatus.Success)
                {
                    executionStatus = _leadService.PostLead(record, mappedLeadFields, person.Result.Id).GetAwaiter().GetResult();
                }
                switch (executionStatus.Status)
                {
                    case PipedriveStatus.Success:
                        return WorkflowExecutionStatus.Completed;
                    case PipedriveStatus.Failed:
                    case PipedriveStatus.Unknown:
                        _logger.Warn<PipedriveWorkflow>("Failed to execute the workflow {WorkflowName} for {FormName} ({FormId})",
                                                        Workflow.Name, e.Form.Name, e.Form.Id);
                        return WorkflowExecutionStatus.Failed;
                    case PipedriveStatus.NotConfigured:
                        _logger.Warn<PipedriveWorkflow>("Could not execute the workflow {WorkflowName} for {FormName} ({FormId}), because workflow is not correctly configured",
                                                        Workflow.Name, e.Form.Name, e.Form.Id);
                        return WorkflowExecutionStatus.NotConfigured;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(executionStatus));
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
            List<MappedPersonField> mappedPersonFields = JsonConvert.DeserializeObject<List<MappedPersonField>>(PersonFieldMappings);

            List<MappedLeadField> mappedLeadFields = JsonConvert.DeserializeObject<List<MappedLeadField>>(LeadFieldMappings);

            var list = new List<Exception>();

            if (mappedPersonFields == null || mappedPersonFields.Count == 0)
            {
                list.Add(new Exception("Missing person mappings"));
            }
            else
            {
                if (mappedPersonFields.Where(p => String.IsNullOrWhiteSpace(p.PipedriveField) || String.IsNullOrWhiteSpace(p.FormField)).Any())
                {

                    list.Add(new Exception("Incomplete person mappings"));
                }

                if (mappedPersonFields
                    .Where(p => !String.IsNullOrWhiteSpace(p.PipedriveField))
                    .GroupBy(p => p.PipedriveField)
                    .Any(p => p.Count() > 1))
                {
                    list.Add(new Exception("Duplicate person mapping in Pipedrive PersonField"));
                }

                if (!mappedPersonFields.Where(p => !String.IsNullOrWhiteSpace(p.PipedriveField) && p.PipedriveField.Equals("name", StringComparison.InvariantCultureIgnoreCase)).Any())
                {

                    list.Add(new Exception("The Pipedrive person name Field is required. Add a Pipedrive person name field to the mapping"));
                }

                if (!mappedPersonFields.Where(p => !String.IsNullOrWhiteSpace(p.PipedriveField) && p.PipedriveField.Equals("email", StringComparison.InvariantCultureIgnoreCase)).Any())
                {

                    list.Add(new Exception("The Pipedrive person email Field is required. Add a Pipedrive person email field to the mapping"));
                }


            }
            if (mappedLeadFields == null || mappedLeadFields.Count == 0)
            {
                list.Add(new Exception("Missing lead mappings"));
            }
            else
            {
                if (mappedLeadFields.Where(p => String.IsNullOrWhiteSpace(p.PipedriveLeadField) || String.IsNullOrWhiteSpace(p.FormField)).Any())
                {

                    list.Add(new Exception("Incomplete lead mappings"));
                }

                if (mappedLeadFields
                    .Where(p => !String.IsNullOrWhiteSpace(p.PipedriveLeadField))
                    .GroupBy(p => p.PipedriveLeadField)
                    .Any(p => p.Count() > 1))
                {
                    list.Add(new Exception("Duplicate lead mapping in Pipedrive LeadField"));
                }

                if (!mappedLeadFields.Where(p => !String.IsNullOrWhiteSpace(p.PipedriveLeadField) && p.PipedriveLeadField.Equals("title", StringComparison.InvariantCultureIgnoreCase)).Any())
                {

                    list.Add(new Exception("The Pipedrive lead title Field is required. Add a Pipedrive lead title field to the mapping"));
                }
            }
            
            return list;
        }
    }
}
