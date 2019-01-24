using Microsoft.Bot.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Betty.Tests
{
    public class BotConfigurationExtensionsTests
    {
        [Fact]
        public void CanGetRequiredService()
        {
            var configuration = new BotConfiguration();

            configuration.ConnectService(new LuisService()
            {
                AppId = Guid.NewGuid().ToString(),
                Name = "Development",
                Region = "WestEurope",
                SubscriptionKey = Guid.NewGuid().ToString()
            });

            var serviceInstance = configuration.GetRequiredService<LuisService>("Development");

            Assert.NotNull(serviceInstance);
        }


        [Fact]
        public void CanGetNamedRequiredService()
        {
            var configuration = new BotConfiguration();

            configuration.ConnectService(new LuisService()
            {
                AppId = Guid.NewGuid().ToString(),
                Name = "Development-luis",
                Region = "WestEurope",
                SubscriptionKey = Guid.NewGuid().ToString()
            });

            var serviceInstance = configuration.GetRequiredService<LuisService>("Development","luis");

            Assert.NotNull(serviceInstance);
        }

        [Fact]
        public void RaisesExceptionForNonExistingRequiredService()
        {
            var configuration = new BotConfiguration();

            configuration.ConnectService(new LuisService()
            {
                AppId = Guid.NewGuid().ToString(),
                Name = "Production",
                Region = "WestEurope",
                SubscriptionKey = Guid.NewGuid().ToString()
            });

            Assert.Throws<KeyNotFoundException>(() =>
            {
                configuration.GetRequiredService<LuisService>("Development");
            });
        }

        [Fact]
        public void ReturnsNullForNonExistingOptionalService()
        {
            var configuration = new BotConfiguration();

            configuration.ConnectService(new LuisService()
            {
                AppId = Guid.NewGuid().ToString(),
                Name = "Production",
                Region = "WestEurope",
                SubscriptionKey = Guid.NewGuid().ToString()
            });

            var serviceInstance = configuration.GetService<LuisService>("Development");

            Assert.Null(serviceInstance);
        }

        [Fact]
        public void CanGetOptionalService()
        {
            var configuration = new BotConfiguration();

            configuration.ConnectService(new LuisService()
            {
                AppId = Guid.NewGuid().ToString(),
                Name = "Production",
                Region = "WestEurope",
                SubscriptionKey = Guid.NewGuid().ToString()
            });

            var serviceInstance = configuration.GetService<LuisService>("Production");

            Assert.NotNull(serviceInstance);
        }

        [Fact]
        public void CanGetNamedOptionalService()
        {
            var configuration = new BotConfiguration();

            configuration.ConnectService(new LuisService()
            {
                AppId = Guid.NewGuid().ToString(),
                Name = "Production-luis",
                Region = "WestEurope",
                SubscriptionKey = Guid.NewGuid().ToString()
            });

            var serviceInstance = configuration.GetService<LuisService>("Production","luis");

            Assert.NotNull(serviceInstance);
        }
    }
}
