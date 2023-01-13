using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Axendo.Umb.Forms.Pipedrive.Web.Core.Models.Responses
{
    public class PostResult<T> : PostResult
    {
       
        public T Result;
    }

    public class PostResult
    {
        public PipedriveStatus Status;
        
    }
}
