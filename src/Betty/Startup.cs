using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Betty
{
    /// <summary>
    /// Configures the ASP.NET Core Runtime environment.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration for the application.</param>
        /// <param name="hostingEnvironment">The hosting environment for the application.</param>
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment, ILoggerFactory loggerFactory)
        {
            this.Configuration = configuration;
            this.HostingEnvironment = hostingEnvironment;
            this.LoggerFactory = loggerFactory;
        }

        /// <summary>
        /// Gets the configuration for the application.
        /// </summary>
        /// <value>The loaded configuration.</value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the hosting environment for the application.
        /// </summary>
        /// <value>The environment the application is hosted in.</value>
        public IHostingEnvironment HostingEnvironment { get; }

        /// <summary>
        /// Gets the logger factory for the application.
        /// </summary>
        public ILoggerFactory LoggerFactory { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940.
        /// </summary>
        /// <param name="services">Application services collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            var logger = LoggerFactory.CreateLogger<BettyBot>();

            var secretKey = Configuration["BotConfigurationSecret"];
            var configFilePath = Path.Combine(HostingEnvironment.ContentRootPath, Configuration["BotConfigurationFile"]);

            var botConfiguration = BotConfiguration.Load(configFilePath, secretKey);
            services.AddSingleton(botConfiguration);

            services.AddSingleton(serviceProvider => new BotServices(
                serviceProvider.GetRequiredService<BotConfiguration>(),
                serviceProvider.GetRequiredService<IHostingEnvironment>()));

            var endpointService = botConfiguration.GetRequiredService<EndpointService>(HostingEnvironment.EnvironmentName);

            services.AddStateStorage(HostingEnvironment, botConfiguration);

            services.AddBot<BettyBot>(config =>
            {
                config.CredentialProvider = new SimpleCredentialProvider(endpointService.AppId, endpointService.AppPassword);

                // Bind a generic error handler for the bot.
                // This error handler simply tells the user that the bot is broken.
                // You could of course tell it in a more gentle way.
                config.OnTurnError = async (context, exception) =>
                {
                    logger.LogError(exception, "Failed to complete turn.");
                    await context.SendActivityAsync("Ooh, I'm sorry, I'm experiencing a brain malfunction. Please hold.");
                };
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">HTTP request pipeline builder.</param>
        /// <param name="env">Hosting environment.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseBotFramework();
        }
    }
}
