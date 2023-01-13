using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axendo.Umb.Forms.Pipedrive.Web.Core.Services
{
    public interface IPipedriveBaseService
    {
        bool GetApiKey(out string apiKey);
        string ApiUrl(string path, string apiKey);

        string SetupApiUrlWithQueryParams(string path, Dictionary<string, string> queryParams, string apiKey);
    }
}
