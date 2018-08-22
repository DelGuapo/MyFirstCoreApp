using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MyFirstCoreApp
{

    public class AJAXify
    {
        private _ajaxify ajax;
        public  AJAXify()
        {
            /* AJAXify is a public facing class that interfaces with a private class (_ajaxify).
             * We needed to do this to inject the HTTPFactory to make it simple for the interfacor.
             * This is equivilent to adding the service in the app Startup with addService()
             */
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddHttpClient();
            
            /* Create an instance of a _ajaxify class (below) and inject
             * the services generated above
             */
            var services = serviceCollection.BuildServiceProvider();
            ajax = ActivatorUtilities.CreateInstance<_ajaxify>(services);
        }

        /* The following public facing procedures interact   are the two externally facing functions
         *  that interact with private _cryptify.
         */
        public async Task<string> get(string url)
        {
            return await ajax._get(url);
        }

        public async Task<string> post(string url, object body = null)
        {
            return await ajax._post(url, body);
        }

        public void addHeader(string name, string value)
        {
            string type = null;
            List<string> contentType = new List<string>();
            contentType.Add("CONTENT-TYPE");

            List<string> accetpsType = new List<string>();
            accetpsType.Add("ACCEPTS");


            if (contentType.Contains(name.ToUpper()))
            {
                type = "content-type";

            } else if (accetpsType.Contains(name.ToUpper()))
            {
                type = "accepts";
            }
            ajax._addHeader(name, value, type);
        }

        


        /* Private facing _cryptophy does the cryptographic work with the injected 
         * DataProtectionProvider.
         */
        private class _ajaxify
        {
            HttpClient _client;
            string contentType = null;
            public _ajaxify(IHttpClientFactory factory)
            {
                _client = factory.CreateClient();
            }

            public void _addHeader(string name, string value, string type = "default")
            {
                switch (type)
                {
                    case "default":
                        _client.DefaultRequestHeaders.Add(name, value);
                        break;
                    case "accepts":
                        // TODO: Add 'accepts' headers ... 
                        break;
                    case "content-type":
                        contentType = value;
                        break;

                }


                try
                {
                    
                }
                catch(Exception err)
                {
                    var s = err.ToString();
                }
                
            }

            public async Task<string> _get(string url)
            {
                var result = await _client.GetStringAsync(url);
                return result;
            }

            public async Task<string> _post(string url, object body = null)
            {
                string content = new Stringify().fromObject(body);
                /* TODO: figure how to change content-type and pipe from public facing class.
                 * https://stackoverflow.com/questions/10679214/how-do-you-set-the-content-type-header-for-an-httpclient-request
                 */
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
                if (contentType != null)
                {
                    request.Content = new StringContent(content, null, contentType);
                }

                var result = await _client.SendAsync(request);
                if (result.IsSuccessStatusCode)
                {
                    return await result.Content.ReadAsStringAsync();
                }
                else
                {
                    return result.ReasonPhrase;
                }
            }
        }

    }
}
