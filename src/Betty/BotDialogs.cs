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
        /// <summary>
        /// Initializes a new instance of the <see cref="BotDialogs"/> class.
        /// </summary>
        /// <param name="dialogState">The dialog state to use.</param>
        public BotDialogs(IStatePropertyAccessor<DialogState> dialogState)
            : base(dialogState)
        {
            Add(CreateRootDialog());
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
                        "Hello and welcome to probabibility spaceflight. " +
                        "Your chance to get to the moon (or not, we're not sure yet).");

                    await stepContext.Context.SendActivityAsync("I'm Betty. How can I help you today?");

                    return await stepContext.ContinueDialogAsync(cancellationToken);
                },
            };

            return new WaterfallDialog(DialogNames.RootDialog, steps);
        }
    }
}
