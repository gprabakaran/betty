using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Betty
{
    /// <summary>
    /// Defines the main structure for the betty bot. All incoming messages are handled here.
    /// Either by using dialogs or through a direct reply.
    /// </summary>
    public class BettyBot : IBot
    {
        private readonly BotDialogs _dialogs;
        private readonly ConversationState _conversationState;

        /// <summary>
        /// Initializes a new instance of the <see cref="BettyBot"/> class.
        /// </summary>
        /// <param name="conversationState">The conversation state for the bot.</param>
        /// <param name="botServices">Bot services to use.</param>
        public BettyBot(ConversationState conversationState, BotServices botServices)
        {
            var dialogStateProperty = conversationState.CreateProperty<DialogState>(nameof(DialogState));

            _dialogs = new BotDialogs(dialogStateProperty, botServices);
            _conversationState = conversationState;
        }

        /// <summary>
        /// Handles a single turn of the chatbot.
        /// </summary>
        /// <param name="turnContext">Context information for the turn.</param>
        /// <param name="cancellationToken">Cancellation token used to stop processing.</param>
        /// <returns>Returns an awaitable task.</returns>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var activity = turnContext.Activity;
            var dialogContext = await _dialogs.CreateContextAsync(turnContext, cancellationToken);

            if (activity.Type == ActivityTypes.ConversationUpdate)
            {
                // Start the root dialog when the user joins the conversation.
                if (turnContext.Activity.MembersAdded.Any(x => x.Id == activity.Recipient.Id))
                {
                    await dialogContext.BeginDialogAsync(DialogNames.RootDialog, null, cancellationToken);
                }
            }
            else if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                await dialogContext.ContinueDialogAsync(cancellationToken);

                // When the dialog tree is finished and has no state, we're not going to get an answer
                // from the dialog. There's not really a good way to resolve this problem, so restart with the root dialog.
                if (!turnContext.Responded)
                {
                    await dialogContext.BeginDialogAsync(DialogNames.RootDialog);
                }
            }

            await _conversationState.SaveChangesAsync(turnContext, cancellationToken: cancellationToken);
        }
    }
}
