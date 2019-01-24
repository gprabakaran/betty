using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Betty.Tests
{
    public class BotServicesTests
    {
        [Fact]
        public void CanLoadLuisService()
        {
            var configuration = new BotConfiguration();
            var hostingEnvironment = CreateHostingEnvironment();

            configuration.ConnectService(new LuisService()
            {
                AppId = Guid.NewGuid().ToString(),
                Name = "luis",
                Region = "WestEurope",
                SubscriptionKey = Guid.NewGuid().ToString()
            });

            var botServices = new BotServices(configuration, hostingEnvironment);

           Assert.Single(botServices.IntentRecognizers);
        }

        [Fact]
        public void CanLoadQnaMakerService()
        {
            var configuration = new BotConfiguration();
            var hostingEnvironment = CreateHostingEnvironment();

            configuration.ConnectService(new QnAMakerService
            {
                SubscriptionKey =Guid.NewGuid().ToString(),
                EndpointKey=Guid.NewGuid().ToString(),
                Hostname = "http://localhost",
                KbId = Guid.NewGuid().ToString(),
                Name = "knowledgebase"
            });

            var botServices = new BotServices(configuration,hostingEnvironment);

            Assert.Single(botServices.Knowledgebases);
        }


        private static IHostingEnvironment CreateHostingEnvironment()
        {
            var hostingEnvironment = new Mock<IHostingEnvironment>();
            hostingEnvironment.Setup(x => x.EnvironmentName).Returns("Development");

            return hostingEnvironment.Object;
        }
    }
}
