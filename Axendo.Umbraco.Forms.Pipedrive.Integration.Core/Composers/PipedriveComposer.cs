using Axendo.Umbraco.Forms.Pipedrive.Core.Services;
using Axendo.Umbraco.Forms.Pipedrive.Core.Workflow;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Forms.Core.Providers;

namespace Axendo.Umbraco.Forms.Pipedrive.Core.Composers
{
    public class PipedriveComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<IPersonService, PipedrivePersonService>();
            builder.Services.AddSingleton<ILeadService, PipedriveLeadService>();

            builder.WithCollectionBuilder<WorkflowCollectionBuilder>()
                .Add<PipedriveWorkflow>();
        }
    }
}
