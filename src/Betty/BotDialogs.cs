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
        }
    }
}
