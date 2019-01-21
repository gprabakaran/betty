using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Betty
{
    /// <summary>
    /// Provides an easy to use extension method to configure various state stores for the bot.
    /// </summary>
    public static class BotStorageExtensions
    {
        /// <summary>
        /// Adds state storage services to the application container.
        /// </summary>
        /// <param name="services">Services collection.</param>
        /// <param name="hostingEnvironment">Hosting environment for the application.</param>
        /// <param name="configuration">Bot configuration to use for initializing the state storage.</param>
        public static void AddStateStorage(this IServiceCollection services, IHostingEnvironment hostingEnvironment, BotConfiguration configuration)
        {
            var storage = CreateBotStorage(configuration, hostingEnvironment);
            var userState = new UserState(storage);
            var conversationState = new ConversationState(storage);

            services.AddSingleton(userState);
            services.AddSingleton(conversationState);
        }

        /// <summary>
        /// Initializes the bot storage.
        /// </summary>
        /// <param name="botConfiguration">Bot configuration to use for setting up the bot storage.</param>
        /// <returns>Returns an initialized instance of the bot storage.</returns>
        private static IStorage CreateBotStorage(BotConfiguration botConfiguration, IHostingEnvironment hostingEnvironment)
        {
            IStorage botStorage = null;

            var storageService = botConfiguration.GetRequiredService<BlobStorageService>(
                hostingEnvironment.EnvironmentName, "storage");

            if (storageService == null)
            {
                botStorage = new MemoryStorage();
            }
            else
            {
                botStorage = new AzureBlobStorage(
                     storageService.ConnectionString,
                     storageService.Container);
            }

            return botStorage;
        }
    }
}
