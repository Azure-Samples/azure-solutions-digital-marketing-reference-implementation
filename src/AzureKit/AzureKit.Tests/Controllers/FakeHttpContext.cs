using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AzureKit.Tests.Controllers
{
    internal class FakeHttpContext : HttpContextBase
    {
        public override HttpResponseBase Response => new FakeResponse();

        private class FakeResponse : HttpResponseBase
        {
            public override void RemoveOutputCacheItem(string path)
            {
                return;
            }
        }
    }
}
