﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Enyim.Caching;
using Microsoft.Extensions.Configuration;

namespace SampleWebApp
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();            
        }

        public IConfigurationRoot Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            var memcachedConfig = Configuration.GetSection("enyimMemcached");
            if (string.IsNullOrEmpty(memcachedConfig.Value))
            {
                services.AddEnyimMemcached(options =>
                {
                    options.AddServer("memcached", 11211);
                });
            }
            else
            {
                services.AddEnyimMemcached(options =>
                {
                    memcachedConfig.Bind(options);
                });
            }
        }

        

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Debug);

            var memcachedClient = app.ApplicationServices.GetService<IMemcachedClient>();
            var logger = loggerFactory.CreateLogger<MemcachedClient>();

            app.Run(async (context) =>
            {
                var cacheKey = "sample_response";
                await memcachedClient.AddAsync(cacheKey, "Hello World!", 60);
                var cacheResult = await memcachedClient.GetAsync<string>(cacheKey);
                if (cacheResult.Success)
                {
                    await context.Response.WriteAsync(cacheResult.Value);
                    await memcachedClient.RemoveAsync(cacheKey);
                    logger.LogDebug($"Hinted cache with '{cacheKey}' key");
                }
                else
                {
                    var message = $"Missed cache with '{cacheKey}' key";
                    await context.Response.WriteAsync(message);
                    logger.LogError(message);
                }
            });
        }
    }
}
