using System;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace Betty
{
    /// <summary>
    /// Defines the set of dialogs supported by the bot.
    /// </summary>
    public class BotDialogs : DialogSet
    {
        private readonly BotServices _botServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="BotDialogs"/> class.
        /// </summary>
        /// <param name="dialogState">The dialog state to use.</param>
        /// <param name="botServices">Bot services needed by the dialogs.</param>
        public BotDialogs(IStatePropertyAccessor<DialogState> dialogState, BotServices botServices)
            : base(dialogState)
        {
            _botServices = botServices;

            Add(CreateRootDialog());
            Add(CreateMenuDialog());

            Add(new TextPrompt(PromptNames.MenuItem));
        }

        /// <summary>
        /// Creates the root dialog for the bot.
        /// </summary>
        /// <returns>Returns the dialog structure.</returns>
        private Dialog CreateRootDialog()
        {
            var steps = new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    await stepContext.Context.SendActivityAsync(
                        "Hello I'm Betty, welcome to improbable air. " +
                        "Your chance to get to the moon (or not, we're not sure yet).");

                    return await stepContext.ReplaceDialogAsync(DialogNames.MainMenuDialog);
                },
            };

            return new WaterfallDialog(DialogNames.RootDialog, steps);
        }

        /// <summary>
        /// Creates the dialog to handle the main menu in the bot.
        /// </summary>
        /// <returns>Returns the dialog structure.</returns>
        private Dialog CreateMenuDialog()
        {
            var steps = new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    var promptOptions = new PromptOptions
                    {
                        Prompt = MessageFactory.Text("How can I help you today?"),
                    };

                    return await stepContext.PromptAsync(PromptNames.MenuItem, promptOptions);
                },
                async (stepContext, cancellationToken) =>
                {
                    var intents = await _botServices.GetRecognizer("luis").RecognizeAsync(
                        stepContext.Context, cancellationToken);

                    var topIntent = intents.GetTopScoringIntent().intent.ToLower();

                    if (topIntent == "checkin")
                    {
                        return await stepContext.ReplaceDialogAsync(DialogNames.CheckinDialog);
                    }

                    if (topIntent == "none")
                    {
                        return await stepContext.ReplaceDialogAsync(DialogNames.FaqDialog);
                    }

                    return await stepContext.ReplaceDialogAsync(DialogNames.MainMenuDialog);
                },
            };

            return new WaterfallDialog(DialogNames.MainMenuDialog, steps);
        }
    }
}
