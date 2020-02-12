using System;
using System.IO;
using System.Reflection;
using arachni.Helper;
using arachni.Managers;
using arachni.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;

namespace arachni
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Register Dependency Injection
            services.AddTransient<IDocumentParser, DocumentParser>();
            services.AddHttpClient<ISpiderService, SpiderService>();
            services.AddTransient<ISpiderManager, SpiderManager>();

            services.AddSwaggerGen(
                options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "Arachni API",
                        Description = "A silly web crawler api",
                        Contact = new OpenApiContact
                        {
                            Name = "overbit",
                            Email = string.Empty,
                            Url = new Uri("https://github.com/overbit/"),
                        }
                    });
                    // integrate xml comments
                    options.IncludeXmlComments(XmlCommentsFilePath());
                });

            services.AddControllers()
                .AddJsonOptions(opt => { opt.JsonSerializerOptions.WriteIndented = true; });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        static string XmlCommentsFilePath()
        {
            var basePath = PlatformServices.Default.Application.ApplicationBasePath;
            var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
            return Path.Combine(basePath, fileName);
        }

    }
}
