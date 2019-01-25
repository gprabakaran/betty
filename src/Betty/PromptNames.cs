using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Betty
{
    /// <summary>
    /// Defines all possible prompt names in use in the bot.
    /// </summary>
    public static class PromptNames
    {
        /// <summary>
        /// A menu option from the main menu.
        /// </summary>
        public const string MenuItem = "menuOptionPrompt";

        /// <summary>
        /// An answer to the security question.
        /// </summary>
        public const string SecurityQuestion = "securityQuestionPrompt";

        /// <summary>
        /// Payment confirmation.
        /// </summary>
        public const string PaymentConfirmation = "paymentConfirmationPrompt";
    }
}
