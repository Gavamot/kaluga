using App.KeywordsSearchService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;

namespace App
{
    public static class ServicesExt
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Kaluga",
                    Version = "v1"
                });

                var xmlFile = $"{ Assembly.GetExecutingAssembly().GetName().Name }.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            return services;
        }


        public static IServiceCollection AddKeywordsService(this IServiceCollection services, Config config)
        {
            services.AddSingleton<IKeywordsSearchConfig>(config);
            services.AddTransient<IKeywordsSearchService, ParallelQueueKeywordsSearchService>();
            
            services.AddScoped<IStackExchangeHttpClient, StackExchangeHttpClient>()
                .AddHttpClient(StackExchangeHttpClient.Name, (serviceProvider, httpClinet) =>
                {
               
                }).ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                    MaxConnectionsPerServer = config.StackExchangeMaxConnections,
                }); 
  
            return services;
        }
    }
}
