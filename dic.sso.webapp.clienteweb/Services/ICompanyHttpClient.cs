using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace dic.sso.webapp.clienteweb.Services
{
    public interface ICompanyHttpClient
    {
        Task<HttpClient> GetClient();
    }
}
