using Microsoft.AspNetCore.Mvc;
using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Authorization;

namespace MapsExample.Controllers
{
    [Authorize]
    public class ApiController : Controller
    {
        private readonly string[] allowed = { "https://navatron-maps.azurewebsites.net/",
                                              "https://localhost"};

        public async Task<IActionResult> GetAzureMapsToken()
        {
            string referer = HttpContext.Request.Headers["Referer"];
            if (string.IsNullOrEmpty(referer))
                return new UnauthorizedResult();

            string result = Array.Find(allowed, site => referer.StartsWith(site));
            if (string.IsNullOrEmpty(result))
                return new UnauthorizedResult();

            // Managed identities for Azure resources and Azure Maps
            // For the Web SDK to authorize correctly, you still must assign Azure role based access control for the managed identity
            // https://docs.microsoft.com/en-us/azure/azure-maps/how-to-manage-authentication
            var tokenCredential = new DefaultAzureCredential();
            var accessToken = await tokenCredential.GetTokenAsync(
                new TokenRequestContext(new[] { "https://atlas.microsoft.com/.default" })
            );

            return new OkObjectResult(accessToken.Token);
        }
    }
}
