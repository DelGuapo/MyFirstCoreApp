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

        public async Task<string> post(string url)
        {
            return await ajax._post(url);
        }

        public void addHeader(string name, string value)
        {
            List<string> contentType = new List<string>();
            contentType.Add("CONTENT-TYPE");


            if (contentType.Contains(name.ToUpper()))
            {
                /* add as media headers */
                

            }
            else
            {
                /* add as default header*/

            }
            

            
            ajax._addHeader(name, value);
        }

        


        /* Private facing _cryptophy does the cryptographic work with the injected 
         * DataProtectionProvider.
         */
        private class _ajaxify
        {
            HttpClient _client;
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
                        _client.DefaultRequestHeaders.Accept
                        break;
                    case "content":
                        _client.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
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

            public async Task<string> _post(string url, )
            {
                var values = new Dictionary<string, string>();
                values.Add("property", "value");
                var content = new FormUrlEncodedContent(values);
                /* TODO: figure how to change content-type and pipe from public facing class.
                 * https://stackoverflow.com/questions/10679214/how-do-you-set-the-content-type-header-for-an-httpclient-request
                 */
                var result = await _client.PostAsync(url,content);
                if (result.IsSuccessStatusCode)
                {
                    return result.Content.ToString();
                }
                else
                {
                    return result.ReasonPhrase;
                }
            }
        }

    }
}
