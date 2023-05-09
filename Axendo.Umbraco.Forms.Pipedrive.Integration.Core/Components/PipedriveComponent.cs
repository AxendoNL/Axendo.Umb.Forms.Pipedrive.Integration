using Axendo.Umbraco.Forms.Pipedrive.Core.Controller;
using Axendo.Umbraco.Forms.Pipedrive.Core.Services;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Extensions;

namespace Axendo.Umbraco.Forms.Pipedrive.Core.Components
{
    public class ServerVariablesParsingComponent : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddNotificationHandler<ServerVariablesParsingNotification, ServerVariablesParsingNotificationHandler>();

            builder.Services.AddSingleton<IPersonService, PipedrivePersonService>();
            builder.Services.AddSingleton<ILeadService, PipedriveLeadService>();
        }
    }

    public class ServerVariablesParsingNotificationHandler : INotificationHandler<ServerVariablesParsingNotification>
    {
        private readonly LinkGenerator linkGenerator;

        public ServerVariablesParsingNotificationHandler(LinkGenerator linkGenerator)
        {
            this.linkGenerator = linkGenerator;
        }

        public void Handle(ServerVariablesParsingNotification notification)
        {
            ServerVariablesParser_Parsing(notification.ServerVariables);
        }

        private void ServerVariablesParser_Parsing(IDictionary<string, object> e)
        {
            if (!e.ContainsKey("umbracoUrls"))
            {
                throw new ArgumentException("Missing umbracoUrls.");
            }

            var umbracoUrlsObject = e["umbracoUrls"];
            if (umbracoUrlsObject == null)
            {
                throw new ArgumentException("Null umbracoUrls");
            }

            if (!(umbracoUrlsObject is Dictionary<string, object> umbracoUrls))
            {
                throw new ArgumentException("Invalid umbracoUrls");
            }
            
            umbracoUrls["umbracoFormsExtensionsPipedriveBaseUrl"] = linkGenerator.GetUmbracoApiServiceBaseUrl<PipedriveController>(controller => controller.GetAllPersonFields()) ?? string.Empty;
            umbracoUrls["umbracoFormsExtensionsPipedriveLeadBaseUrl"] = linkGenerator.GetUmbracoApiServiceBaseUrl<PipedriveController>(controller => controller.GetAllLeadFields()) ?? string.Empty;
        }
    }
}
