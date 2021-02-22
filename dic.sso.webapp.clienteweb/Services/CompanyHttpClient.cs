using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace dic.sso.webapp.clienteweb.Services
{
    public class CompanyHttpClient : ICompanyHttpClient
    {
        private HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CompanyHttpClient(IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = new HttpClient();
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<HttpClient> GetClient()
        {
            try
            {
                var accessToken = await _httpContextAccessor
                       .HttpContext
                       .GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    _httpClient.SetBearerToken(accessToken);
                }

                _httpClient.BaseAddress = new Uri("http://localhost:5002/");
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                return _httpClient;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
