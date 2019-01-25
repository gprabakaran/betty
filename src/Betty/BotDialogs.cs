using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Betty
{
    /// <summary>
    /// Defines the set of dialogs supported by the bot.
    /// </summary>
    public class BotDialogs : DialogSet
    {
        private readonly BotServices _botServices;
        private readonly IStatePropertyAccessor<ConversationData> _conversationData;

        /// <summary>
        /// Initializes a new instance of the <see cref="BotDialogs"/> class.
        /// </summary>
        /// <param name="dialogState">The dialog state to use.</param>
        /// <param name="conversationData">The conversation data.</param>
        /// <param name="botServices">Bot services needed by the dialogs.</param>
        /// <param name="scale">The luggage scale.</param>
        public BotDialogs(
            IStatePropertyAccessor<DialogState> dialogState,
            IStatePropertyAccessor<ConversationData> conversationData,
            BotServices botServices, 
            ILuggageScale scale)
            : base(dialogState)
        {
            _botServices = botServices;
            _conversationData = conversationData;

            Add(CreateRootDialog());
            Add(CreateMenuDialog());
            Add(CreateCheckinDialog());
            Add(CreateSecurityQuestionDialog());
            Add(CreateHeavyLuggageCheckDialog(scale));
            Add(CreateThanksDialog());
            Add(CreatePaymentDialog());
            Add(CreateExtendedSecurityCheckDialog());

            Add(new TextPrompt(PromptNames.MenuItem));
            Add(new TextPrompt(PromptNames.SecurityQuestion));
            Add(new TextPrompt(PromptNames.PaymentConfirmation));
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
                    var data = await _conversationData.GetAsync(
                        stepContext.Context,
                        () => new ConversationData(),
                        cancellationToken);

                    var promptOptions = new PromptOptions
                    {
                        Prompt = MessageFactory.Text(data.ReturningCustomer ?
                        "Is there anything else I can help you with?" :
                        "How can I help you today?"),
                    };

                    // Flag the customer so that we know he/she was here before.
                    // Store the result in the conversation state property.
                    data.ReturningCustomer = true;
                    await _conversationData.SetAsync(stepContext.Context, data);

                    return await stepContext.PromptAsync(PromptNames.MenuItem, promptOptions);
                },
                async (stepContext, cancellationToken) =>
                {
                    var intents = await _botServices.GetRecognizer("luis").RecognizeAsync(
                        stepContext.Context, cancellationToken);

                    var topIntent = intents.GetTopScoringIntent().intent.ToLower();

                    if (topIntent == "checkin")
                    {
                        return await stepContext.BeginDialogAsync(DialogNames.CheckinDialog);
                    }

                    if (topIntent == "none")
                    {
                        return await stepContext.BeginDialogAsync(DialogNames.FaqDialog);
                    }

                    return await stepContext.ReplaceDialogAsync(DialogNames.MainMenuDialog);
                },
                async (stepContext, cancellationToken) =>
                {
                    return await stepContext.ReplaceDialogAsync(DialogNames.MainMenuDialog);
                },
            };

            return new WaterfallDialog(DialogNames.MainMenuDialog, steps);
        }

        /// <summary>
        /// Creates the checkin dialog.
        /// </summary>
        /// <returns>Returns the dialog structure.</returns>
        private Dialog CreateCheckinDialog()
        {
            var steps = new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    await stepContext.Context.SendActivityAsync("Can I have your passport and boarding pass please?");

                    // TODO: allow user to upload two images of passport and boarding pass. Left out for demo purposes
                    return await stepContext.BeginDialogAsync(DialogNames.SecurityQuestionDialog);
                },
            };

            return new WaterfallDialog(DialogNames.CheckinDialog, steps);
        }

        /// <summary>
        /// Creates the dialog for asking the most important question of the check-in process.
        /// </summary>
        /// <returns>Returns the dialog structure.</returns>
        private Dialog CreateSecurityQuestionDialog()
        {
            var steps = new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    var promptOptions = new PromptOptions
                    {
                        Prompt = MessageFactory.Text("Did you pack this luggage yourself?"),
                    };

                    return await stepContext.PromptAsync(PromptNames.SecurityQuestion, promptOptions);
                },
                async (stepContext, cancellationToken) =>
                {
                    var securityAnswer = stepContext.Result.ToString();
                    var intents = await _botServices.GetRecognizer("luis").RecognizeAsync(stepContext.Context, cancellationToken);
                    var topIntent = intents.GetTopScoringIntent().intent.ToLower();

                    if (topIntent == "confirm")
                    {
                        return await stepContext.ReplaceDialogAsync(
                            DialogNames.CheckHeavyLuggageDialog);
                    }
                    else if (topIntent == "decline")
                    {
                        return await stepContext.ReplaceDialogAsync(
                            DialogNames.ExtendedSecurityCheckDialog);
                    }
                    else
                    {
                        // Restart the dialog when the answer is not satisfactory.
                        return await stepContext.ReplaceDialogAsync(
                            DialogNames.SecurityQuestionDialog,
                            cancellationToken: cancellationToken);
                    }
                },
            };

            return new WaterfallDialog(DialogNames.SecurityQuestionDialog, steps);
        }

        /// <summary>
        /// Creates a dialog to check the weight of the placed luggage.
        /// </summary>
        /// <returns>Returns the dialog structure.</returns>
        private Dialog CreateHeavyLuggageCheckDialog(ILuggageScale scale)
        {
            var steps = new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    var weight = await scale.GetWeight();

                    if (weight > 20.0)
                    {
                        return await stepContext.ReplaceDialogAsync(DialogNames.PaymentDialog);
                    }
                    else
                    {
                        return await stepContext.ReplaceDialogAsync(DialogNames.ThanksDialog);
                    }
                },
            };

            return new WaterfallDialog(DialogNames.CheckHeavyLuggageDialog, steps);
        }

        /// <summary>
        /// Creates a dialog to thank the customer for their time.
        /// </summary>
        /// <returns>Returns the dialog structure.</returns>
        private Dialog CreateThanksDialog()
        {
            var steps = new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    await stepContext.Context.SendActivityAsync("Thanks, have a pleasant flight!");
                    return await stepContext.EndDialogAsync();
                },
            };

            return new WaterfallDialog(DialogNames.ThanksDialog, steps);
        }

        private Dialog CreatePaymentDialog()
        {
            var steps = new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    var paymentCard = new HeroCard(
                        title: "Additional payment requested",
                        subtitle: "50 Intergallactic credits",
                        text: "Extra heavy luggage");

                    paymentCard.Buttons = new List<CardAction>
                    {
                        new CardAction("accept", "Okay"),
                        new CardAction("decline", "No way!"),
                    };

                    var promptOptions = new PromptOptions
                    {
                        Prompt = (Activity)MessageFactory.Attachment(
                            paymentCard.ToAttachment(),
                            text: "Your suitcase is too heavy. We'll have to charge you 50 intergallactic credits. Is this okay?"),
                    };

                    return await stepContext.PromptAsync(PromptNames.PaymentConfirmation, promptOptions);
                },
                async (stepContext, cancellationToken) =>
                {
                    var confirmationData = stepContext.Result.ToString().ToLower();

                    if(confirmationData != "confirm")
                    {
                        await stepContext.Context.SendActivityAsync("We're sorry to inform you that your luggage will be incinerated.");
                    }

                    return await stepContext.ReplaceDialogAsync(DialogNames.ThanksDialog);
                },
            };

            return new WaterfallDialog(DialogNames.PaymentDialog, steps);
        }

        /// <summary>
        /// Creates the extended security check dialog.
        /// </summary>
        /// <returns>Returns the dialog structure.</returns>
        private Dialog CreateExtendedSecurityCheckDialog()
        {
            var steps = new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    var card = new ReceiptCard
                    {
                        Title = "Extended security check",
                        Items = new List<ReceiptItem>
                        {
                            new ReceiptItem(title: "Luggage check", quantity: "1"),
                            new ReceiptItem(title: "Extended personal security check", quantity: "1"),
                        },
                        Tax = "12 credits",
                        Total = "75 credits",
                    };

                    var receiptMessage = MessageFactory.Attachment(
                        card.ToAttachment(),
                        "Okay, well, here's a receipt, please follow Al for an additional security check.");

                    await stepContext.Context.SendActivityAsync(receiptMessage);
                    await stepContext.Context.SendActivityAsync("Have a pleasant security check!");

                    return await stepContext.EndDialogAsync();
                },
            };

            return new WaterfallDialog(DialogNames.ExtendedSecurityCheckDialog, steps);
        }
    }
}
