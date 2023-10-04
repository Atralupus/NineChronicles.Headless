using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NineChronicles.Headless.Services.AccessControlService;

namespace NineChronicles.Headless
{
    public class AcsService
    {
        private StandaloneContext StandaloneContext { get; }

        public AcsService(StandaloneContext standaloneContext)
        {
            StandaloneContext = standaloneContext;
        }

        public IHostBuilder Configure(IHostBuilder hostBuilder, int port)
        {
            return hostBuilder.ConfigureWebHostDefaults(builder =>
            {
                builder.UseStartup(x => new RestApiStartup(x.Configuration, StandaloneContext));
                builder.ConfigureKestrel(options =>
                {
                    options.ListenAnyIP(
                        port,
                        listenOptions =>
                        {
                            listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                        }
                    );
                });
            });
        }

        internal class RestApiStartup
        {
            public RestApiStartup(IConfiguration configuration, StandaloneContext standaloneContext)
            {
                Configuration = configuration;
                StandaloneContext = standaloneContext;
            }

            public IConfiguration Configuration { get; }
            public StandaloneContext StandaloneContext;

            public void ConfigureServices(IServiceCollection services)
            {
                services.AddControllers();

                services.AddSingleton(
                    provider =>
                        AccessControlServiceFactory.CreateAccessControlService(
                            AccessControlServiceFactory.StorageType.Memory,
                            null,
                            null
                        )
                );
            }

            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app.UseRouting();
                app.UseAuthorization();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            }
        }
    }
}
