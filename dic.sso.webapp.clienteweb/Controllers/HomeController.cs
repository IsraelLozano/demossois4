using dic.sso.webapp.clienteweb.Models;
using dic.sso.webapp.clienteweb.Services;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace dic.sso.webapp.clienteweb.Controllers
{

    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICompanyHttpClient _companyHttpClient;

        public HomeController(ILogger<HomeController> logger, ICompanyHttpClient companyHttpClient)
        {
            _logger = logger;
            _companyHttpClient = companyHttpClient;
        }



        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                string accessToken = await HttpContext.GetTokenAsync("access_token");

                // if you need to check the access token expiration time, use this value
                // provided on the authorization response and stored.
                // do not attempt to inspect/decode the access token
                DateTime accessTokenExpiresAt = DateTime.Parse(
                    await HttpContext.GetTokenAsync("expires_at"),
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind);

                string idToken = await HttpContext.GetTokenAsync("id_token");

                // Now you can use them. For more info on when and how to use the
                // access_token and id_token, see https://auth0.com/docs/tokens
            }


            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            // Get the name
            string name = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value;


            var lista = new List<CompanyViewModel>
            {
                new CompanyViewModel{ Id = Guid.NewGuid(), Name="Nombre 1", FullAddress = "xxxxxx" },
                new CompanyViewModel{ Id = Guid.NewGuid(), Name="Nombre 2", FullAddress = "qweqweqweqweqwe" }
            };

            return View(lista);

            //var httpClient = await _companyHttpClient.GetClient();

            //var response = await httpClient.GetAsync("api/companies").ConfigureAwait(false);

            //if (response.IsSuccessStatusCode)
            //{
            //    var companiesString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            //    var companyViewModel = JsonConvert.DeserializeObject<List<CompanyViewModel>>(companiesString).ToList();

            //    return View(companyViewModel);
            //}
            //else if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            //{
            //    return RedirectToAction("AccessDenied", "Account");
            //}

            //throw new Exception($"Problem with fetching data from the API: {response.ReasonPhrase}");

            //return View(lista);
        }



        //[Authorize(Roles = "Admin")]
        [Authorize(Policy = "CanCreateAndModifyData")]
        public async Task<IActionResult> Privacy()
        {
            var client = new HttpClient();
            var metaDataResponse = await client.GetDiscoveryDocumentAsync("http://localhost:5001");

            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            var response = await client.GetUserInfoAsync(new UserInfoRequest
            {
                Address = metaDataResponse.UserInfoEndpoint,
                Token = accessToken
            });

            if (response.IsError)
            {
                throw new Exception("Problem while fetching data from the UserInfo endpoint", response.Exception);
            }

            var addressClaim = response.Claims.FirstOrDefault(c => c.Type.Equals("address"));

            User.AddIdentity(new ClaimsIdentity(new List<Claim> { new Claim(addressClaim.Type.ToString(), addressClaim.Value.ToString()) }));

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

