using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Betty
{
    /// <summary>
    /// Defines constants for the different dialogs supported by the bot.
    /// </summary>
    public static class DialogNames
    {
        /// <summary>
        /// The root dialog.
        /// </summary>
        public const string RootDialog = "rootDialog";

        /// <summary>
        /// The menu dialog.
        /// </summary>
        public const string MainMenuDialog = "menuDialog";

        /// <summary>
        /// FAQ dialog.
        /// </summary>
        public const string FaqDialog = "faqDialog";

        /// <summary>
        /// Check-in dialog.
        /// </summary>
        public const string CheckinDialog = "checkinDialog";

        /// <summary>
        /// Security question.
        /// </summary>
        public const string SecurityQuestionDialog = "securityQuestionDialog";

        /// <summary>
        /// The check and handling of heavy luggage.
        /// </summary>
        public const string CheckHeavyLuggageDialog = "checkHeavyLuggageDialog";

        /// <summary>
        /// The extended security check by Al or Ian.
        /// </summary>
        public const string ExtendedSecurityCheckDialog = "extendedSecurityCheckDialog";

        /// <summary>
        /// The payment dialog.
        /// </summary>
        public const string PaymentDialog = "paymentDialog";

        /// <summary>
        /// The thanks dialog.
        /// </summary>
        public const string ThanksDialog = "thanksDialog";
    }
}
