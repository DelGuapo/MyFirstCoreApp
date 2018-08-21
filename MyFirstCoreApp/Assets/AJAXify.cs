using System;
using System.Net;
using System.Net.Mail;
using System.Collections.Generic;
using System.Linq;
using MailMessage = System.Net.Mail.MailMessage;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace MyFirstCoreApp
{
    /// <summary>
    /// services.AddHttpClient();// Add this line to your Startup.ConfigureServices
    /// Download Microsoft.Extensions.Http  (NUGET)
    /// </summary>
    class AJAXify
    {
        private readonly IHttpClientFactory _clientFactory;
        public bool GetBranchesError { get; private set; }
        public AJAXify(string url)
        {

        }
        public async Task<string> RequestGet()
        {
            string url = "http://127.0.0.1:5984/users";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            //request.Headers.Add("Accept", "application/vnd.github.v3+json");
            //request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

            var client = _clientFactory.CreateClient("TEST");

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                //string rsp = await response.Content.ReadAsStringAsync();
                //return rsp;

                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return "";
                //GetBranchesError = true;
                //Branches = Array.Empty<GitHubBranch>();
            }
        }
    }
}
