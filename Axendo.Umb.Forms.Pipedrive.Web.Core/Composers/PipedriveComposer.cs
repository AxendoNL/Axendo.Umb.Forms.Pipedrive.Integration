using Umbraco.Core;
using Umbraco.Core.Composing;
using Axendo.Umb.Forms.Pipedrive.Web.Core.Services;
using Axendo.Umb.Forms.Pipedrive.Web.Core.Components;

namespace Axendo.Umb.Forms.Pipedrive.Web.Core.Composers
{
    public class PipedriveComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<IPersonService, PipedrivePersonService>(Lifetime.Singleton);
            composition.Register<ILeadService, PipedriveLeadService>(Lifetime.Singleton);
           

            composition.Components().Append<PipedriveComponent>();
            

        }

    }
}
