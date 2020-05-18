using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace DFC.App.JobCategories.PageService
{
    public interface IFakeHttpRequestSender
    {
        HttpResponseMessage Send(HttpRequestMessage request);
    }
}
