using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;

namespace Betty
{
    /// <summary>
    /// Defines the main structure for the betty bot. All incoming messages are handled here.
    /// Either by using dialogs or through a direct reply.
    /// </summary>
    public class BettyBot : IBot
    {
        /// <summary>
        /// Handles a single turn of the chatbot.
        /// </summary>
        /// <param name="turnContext">Context information for the turn.</param>
        /// <param name="cancellationToken">Cancellation token used to stop processing.</param>
        /// <returns>Returns an awaitable task.</returns>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await turnContext.SendActivityAsync("Hello, I'm Betty!");
        }
    }
}
