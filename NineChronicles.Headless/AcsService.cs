using System;
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
        private readonly string _acsType;
        private readonly string? _acsConnectionString;
        private readonly string? _acsInitialBlocklist;

        public AcsService(string acsType, string? acsConnectionString, string? acsInitialBlocklist)
        {
            _acsType = acsType;
            _acsConnectionString = acsConnectionString;
            _acsInitialBlocklist = acsInitialBlocklist;
        }

        public IHostBuilder Configure(IHostBuilder hostBuilder, int port)
        {
            return hostBuilder.ConfigureWebHostDefaults(builder =>
            {
                builder.UseStartup(
                    x =>
                        new RestApiStartup(
                            x.Configuration,
                            _acsType,
                            _acsConnectionString,
                            _acsInitialBlocklist
                        )
                );
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
            private readonly string _acsType;
            private readonly string? _acsConnectionString;
            private readonly string? _acsInitialBlocklist;

            public RestApiStartup(
                IConfiguration configuration,
                string acsType,
                string? acsConnectionString,
                string? acsInitialBlocklist
            )
            {
                Configuration = configuration;
                _acsType = acsType;
                _acsConnectionString = acsConnectionString;
                _acsInitialBlocklist = acsInitialBlocklist;
            }

            public IConfiguration Configuration { get; }

            public void ConfigureServices(IServiceCollection services)
            {
                services.AddControllers();

                var accessControlService = AccessControlServiceFactory.CreateAccessControlService(
                    Enum.Parse<AccessControlServiceFactory.StorageType>(_acsType, true),
                    _acsConnectionString,
                    _acsInitialBlocklist
                );

                services.AddSingleton(accessControlService);
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
