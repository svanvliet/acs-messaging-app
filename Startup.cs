using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SVV.MessagingApp.Data;

namespace SVV.MessagingApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddMvc();
            
            // Adding a singleton instance of the MessageThreadService for injection and use in our Razor pages
            services.AddSingleton<MessageThreadService>();

            // Adding SignalR support so that we can handle webhook calls from EventGrid. Note that we also need to handle
            // the compression MIME type so that we can decompress the JSON payload from EventGrid calls
            services.AddSignalR();
            services.AddResponseCompression(opts => {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes
                    .Concat(new []{"application/octet-stream"});
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                // Mappin the SignalR hubs we need to handle and process the events received from EventGrid.
                // For purposes of this demo, we are only handling two events - Validation (which is required
                // to wire-up on the EventGrid sid), and the inbound SMS event.
                endpoints.MapBlazorHub();
                endpoints.MapHub<Hubs.SmsHub>("smshub");
                
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
