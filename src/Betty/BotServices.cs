using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Configuration;

namespace Betty
{
    /// <summary>
    /// Use this to get access to various bot related services in the application.
    /// </summary>
    public class BotServices
    {
        private readonly BotConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;

        /// <summary>
        /// Initializes a new instance of the <see cref="BotServices"/> class.
        /// </summary>
        /// <param name="configuration">Bot configuration to load service settings from.</param>
        /// <param name="hostingEnvironment">Hosting environment for the application.</param>
        public BotServices(BotConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;

            foreach (var service in configuration.Services)
            {
                switch (service.Type)
                {
                    case ServiceTypes.Luis:
                        RegisterIntentRecognizer(service);
                        break;
                    case ServiceTypes.QnA:
                        RegisterKnowledgebase(service);
                        break;
                }
            }
        }

        /// <summary>
        /// Gets the collection of known LUIS models in the application.
        /// </summary>
        /// <value>The loaded LUIS models.</value>
        public Dictionary<string, LuisRecognizer> IntentRecognizers { get; } = new Dictionary<string, LuisRecognizer>();

        /// <summary>
        /// Gets the configured knowledgebases in the application.
        /// </summary>
        /// <value>The loaded knowledgebases.</value>
        public Dictionary<string, QnAMaker> Knowledgebases { get; } = new Dictionary<string, QnAMaker>();

        /// <summary>
        /// Gets an intent recognizer based on the current environment and name.
        /// </summary>
        /// <param name="key">The name of the LUIS model.</param>
        /// <returns>Returns the found model.</returns>
        public LuisRecognizer GetRecognizer(string key)
        {
            return IntentRecognizers[$"{_hostingEnvironment.EnvironmentName}-{key}"];
        }

        /// <summary>
        /// Gets a knowledge base based on the current environment and name.
        /// </summary>
        /// <param name="key">The name of the knowledge base.</param>
        /// <returns>Returns the found knowledge base.</returns>
        public QnAMaker GetKnowledgebase(string key)
        {
            return Knowledgebases[$"{_hostingEnvironment.EnvironmentName}-{key}"];
        }

        /// <summary>
        /// Registers a new knowledge base instance.
        /// </summary>
        /// <param name="service">Service instance to register.</param>
        private void RegisterKnowledgebase(ConnectedService service)
        {
            var qnaMakerService = (QnAMakerService)service;

            var endpoint = new QnAMakerEndpoint
            {
                EndpointKey = qnaMakerService.EndpointKey,
                Host = qnaMakerService.Hostname,
                KnowledgeBaseId = qnaMakerService.KbId,
            };
            
            var qna = new QnAMaker(endpoint);

            Knowledgebases.Add(qnaMakerService.Name, qna);
        }

        /// <summary>
        /// Registers a new LUIS intent recognizer instance.
        /// </summary>
        /// <param name="service">Service instance to register.</param>
        private void RegisterIntentRecognizer(ConnectedService service)
        {
            var luisService = (LuisService)service;

            var app = new LuisApplication(
                luisService.AppId,
                luisService.SubscriptionKey,
                luisService.GetEndpoint());

            var recognizer = new LuisRecognizer(app);

            IntentRecognizers.Add(luisService.Name, recognizer);
        }
    }
}
