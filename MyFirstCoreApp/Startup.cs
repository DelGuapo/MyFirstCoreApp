using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace MyFirstCoreApp
{
    public class Startup
    {
        /// <summary>
        /// To the Developer: FILL THIS OUT
        /// </summary>
        private string APIName = "MyFirstCoreApp";
        private string APIRepo = "https://github.com/DelGuapo/MyFirstCoreApp";
        private string APIOwner = "delGuapo";
        private string APIDescrip = 
@"This is a boilerplate .net-core app that demonstrates several featurs. 
Not all routes will work out of the box because they assume you have
external services provided (SQL Server, Couch DB, etc..).  However, the 
code is descriptive enough to get you started with your own project.

<h3>Deployment Resources</h3>
<a href='https://www.microsoft.com/net/learn/dotnet/hello-world-tutorial#linuxubuntu%22'>Setup .NET-CORE</a>
<a href='https://www.digitalocean.com/community/tutorials/how-to-install-nginx-on-ubuntu-18-04'>Setup NGINX</a>
<a href='https://linuxconfig.org/how-to-enable-disable-firewall-on-ubuntu-18-04-bionic-beaver-linux'>Enable Firewall</a>
";



        private static System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
        private static FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
        private static string APIVersion = fvi.FileMajorPart + "." + fvi.FileMinorPart;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddHttpClient(); // <<== REQUIRES Microsoft.Extensions.Http  (NUGET)

            /* TO ADD SPECIFIC SERVER CONNECTIONS ADD HERE: */
            services.AddHttpClient("couch", c =>
            {
                c.BaseAddress = new Uri("http://127.0.0.1:5984/");
                c.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
            });

            /* TO ENABLE SWAGGER: 
             * https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-2.1&tabs=visual-studio%2Cvisual-studio-xml
             */
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(APIVersion, new Info {
                    Title = APIName,
                    Version = APIVersion,
                    Description = APIDescrip,
                    Contact = new Contact
                    {
                        Name = APIOwner,
                        Email = string.Empty,
                        Url = APIRepo
                    },
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/" + APIVersion + "/swagger.json", APIName);
                //c.RoutePrefix = string.Empty;
            });


            app.UseMvc();
        }
    }
}
