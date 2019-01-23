using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Configuration;

namespace Betty
{
    /// <summary>
    /// Extension methods for the <see cref="BotConfiguration"/>.
    /// </summary>
    public static class BotConfigurationExtensions
    {
        /// <summary>
        /// Finds a required service in the bot configuration.
        /// </summary>
        /// <typeparam name="T">Type of service to locate.</typeparam>
        /// <param name="configuration">Configuration to load from.</param>
        /// <param name="environment">Environment the bot is hosted.</param>
        /// <returns>Returns the located service instance.</returns>
        public static T GetRequiredService<T>(this BotConfiguration configuration, string environment)
            where T : ConnectedService
        {
            return GetRequiredService<T>(configuration, environment, null);
        }

        /// <summary>
        /// Finds a required service in the bot configuration.
        /// </summary>
        /// <typeparam name="T">Type of service to locate.</typeparam>
        /// <param name="configuration">Configuration to load from.</param>
        /// <param name="environment">Environment the bot is hosted.</param>
        /// <param name="name">Name of the service (optional).</param>
        /// <returns>Returns the located service instance.</returns>
        public static T GetRequiredService<T>(this BotConfiguration configuration, string environment, string name)
            where T : ConnectedService
        {
            var serviceInstance = GetService<T>(configuration, environment, name);

            if (serviceInstance == null)
            {
                throw new KeyNotFoundException($"Could not find service with name {name} for environment {environment}");
            }

            return serviceInstance;
        }

        /// <summary>
        /// Finds a service in the bot configuration.
        /// </summary>
        /// <typeparam name="T">Type of service to locate.</typeparam>
        /// <param name="configuration">Configuration to load from.</param>
        /// <param name="environment">Environment the bot is hosted.</param>
        /// <returns>Returns the located service instance.</returns>
        public static T GetService<T>(this BotConfiguration configuration, string environment)
            where T : ConnectedService
        {
            return GetService<T>(configuration, environment, null);
        }

        /// <summary>
        /// Finds a service in the bot configuration.
        /// </summary>
        /// <typeparam name="T">Type of service to locate.</typeparam>
        /// <param name="configuration">Configuration to load from.</param>
        /// <param name="environment">Environment the bot is hosted.</param>
        /// <param name="name">Name of the service (optional).</param>
        /// <returns>Returns the located service instance.</returns>
        public static T GetService<T>(this BotConfiguration configuration, string environment, string name)
            where T : ConnectedService
        {
            var serviceName = BuildServiceName(environment, name);
            var serviceType = TranslateServiceType(typeof(T));

            var serviceInstance = (T)configuration.Services.FirstOrDefault(x => x.Type == serviceType && x.Name == serviceName);

            return serviceInstance;
        }

        /// <summary>
        /// Translates the type information into a constant for the bot configuration locator.
        /// </summary>
        /// <param name="serviceType">CLR type to resolve.</param>
        /// <returns>Returns the resolved service type name constant.</returns>
        private static string TranslateServiceType(Type serviceType)
        {
            if (serviceType == typeof(EndpointService))
            {
                return ServiceTypes.Endpoint;
            }

            if (serviceType == typeof(BlobStorageService))
            {
                return ServiceTypes.BlobStorage;
            }

            if (serviceType == typeof(LuisService))
            {
                return ServiceTypes.Luis;
            }

            if (serviceType == typeof(QnAMakerService))
            {
                return ServiceTypes.QnA;
            }

            throw new ArgumentException("Invalid service type provided.", "serviceType");
        }

        /// <summary>
        /// Builds a fully qualified service name based on the environment and service name.
        /// </summary>
        /// <param name="environment">Environment the bot is hosted in.</param>
        /// <param name="name">Name of the service.</param>
        /// <returns>Returns the fully qualified service name.</returns>
        private static string BuildServiceName(string environment, string name)
        {
            if (name == null)
            {
                return environment;
            }

            return $"{environment}-{name}";
        }
    }
}
