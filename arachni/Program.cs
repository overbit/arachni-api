using System;
using arachni.Configurations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace arachni
{
    public class Program
    { 
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        // This is not the best approach but it was done due to time restriction. 
                        // An asynchronous approach should be taken instead maybe with a callback to the client once the crawl result are done.
                        options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(KestrelConfiguration.KestrelDefaultTimeoutInSeconds);
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
