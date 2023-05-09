using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Axendo.Umbraco.Forms.Pipedrive.Core.Models.Responses
{
    public class PostResult<T> : PostResult where T : new()
    {
        public T Result = new();
    }

    public class PostResult
    {
        public PipedriveStatus Status = PipedriveStatus.Unknown;
    }
}
