using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
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
        private readonly BotServices _botServices;
        private readonly IStatePropertyAccessor<ConversationData> _conversationDataProperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="BettyBot"/> class.
        /// </summary>
        /// <param name="conversationState">The conversation state for the bot.</param>
        /// <param name="botServices">Bot services to use.</param>
        /// <param name="scale">Scale to weigh the luggage.</param>
        public BettyBot(ConversationState conversationState, BotServices botServices, ILuggageScale scale)
        {
            var dialogStateProperty = conversationState.CreateProperty<DialogState>(nameof(DialogState));
            _conversationDataProperty = conversationState.CreateProperty<ConversationData>(nameof(ConversationData));

            _dialogs = new BotDialogs(
                dialogStateProperty,
                _conversationDataProperty,
                botServices,
                scale);

            _conversationState = conversationState;
            _botServices = botServices;
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
                var redirectedToManager = await HandleEscalationProtocol(turnContext, dialogContext, cancellationToken);

                if (!redirectedToManager)
                {
                    var handledWithKnowledgeBase = await HandleWithKnowledgeBase(turnContext, cancellationToken);

                    if (!handledWithKnowledgeBase)
                    {
                        await HandleWithDialog(turnContext, dialogContext, cancellationToken);
                    }
                }
            }

            await _conversationState.SaveChangesAsync(turnContext, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Handles the escalation intent through LUIS.
        /// </summary>
        /// <param name="turnContext">Turn context for the bot.</param>
        /// <param name="dialogContext">Dialog context for the bot.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Returns true when escalated to manager; Otherwise false.</returns>
        private async Task<bool> HandleEscalationProtocol(
            ITurnContext turnContext, 
            DialogContext dialogContext, 
            CancellationToken cancellationToken)
        {
            var intentRecognizer = _botServices.GetRecognizer("luis");
            var intents = await intentRecognizer.RecognizeAsync(turnContext, cancellationToken);
            var topIntent = intents.GetTopScoringIntent();

            // Detect whether the user wants to escalate the conversation to a manager.
            // This is where we provide an escape from the regular route.
            if (topIntent.intent == "escalate" && topIntent.score >= 0.7)
            {
                await turnContext.SendActivityAsync(
                    "Euhm, okay, but Al is not around to help right now. " +
                    "You can write down your complaint and dump it in the idea box over here.");

                await dialogContext.CancelAllDialogsAsync(cancellationToken);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Handles the incoming message through the knowledge base.
        /// </summary>
        /// <param name="turnContext">Turn context for the bot.</param>
        /// <param name="cancellationToken">Cancellation token for the bot.</param>
        /// <returns>Returns true when the knowledge base was used to answer; Otherwise false.</returns>
        private async Task<bool> HandleWithKnowledgeBase(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var knowledgebase = _botServices.GetKnowledgebase("qna");
            var knowledgebaseOptions = new QnAMakerOptions
            {
                ScoreThreshold = 0.7f,
                Top = 1,
            };

            var answers = await knowledgebase.GetAnswersAsync(turnContext, knowledgebaseOptions);

            if (answers != null && answers.Length > 0)
            {
                await turnContext.SendActivityAsync(answers[0].Answer);

                var conversationData = await _conversationDataProperty.GetAsync(
                    turnContext,
                    () => new ConversationData(),
                    cancellationToken);

                conversationData.ReturningCustomer = true;

                await _conversationDataProperty.SetAsync(
                    turnContext,
                    conversationData,
                    cancellationToken);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Handles the incoming activity with a dialog.
        /// </summary>
        /// <param name="turnContext">Turn context for the bot.</param>
        /// <param name="dialogContext">Dialog context for the bot.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Returns an awaitable task.</returns>
        private async Task HandleWithDialog(ITurnContext turnContext, DialogContext dialogContext, CancellationToken cancellationToken)
        {
            await dialogContext.ContinueDialogAsync(cancellationToken);

            // When the dialog tree is finished and has no state, we're not going to get an answer
            // from the dialog. There isn't a good way to resolve this problem, so restart with the root dialog.
            if (!turnContext.Responded)
            {
                await dialogContext.BeginDialogAsync(DialogNames.RootDialog);
            }
        }
    }
}
