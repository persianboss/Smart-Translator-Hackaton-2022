using System.Xml.Linq;
using AdaptiveCards;
using AdaptiveCards.Templating;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TranslatorExtension.CognetiveService;
using TranslatorExtension.CognetiveService.Model;
using static System.Net.Mime.MediaTypeNames;

namespace TranslatorExtension.Bot;

public class TeamsMessageExtension : TeamsActivityHandler
{
    private ITranslationService _translationService { get; }

    // Message Extension Code
    // Action.
    //testcognetiveservice@outlook.com
    private readonly string _cards = Path.Combine(".", "Resources", "TranslationCard.json");

    public TeamsMessageExtension(ITranslationService translationService)
    {
        _translationService = translationService;
    }
    protected override Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionSubmitActionAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
    {
        switch (action.CommandId)
        {
            case "translateMessage":
                return Task.FromResult(TranslateMessage(turnContext, action));
        }
        return Task.FromResult(new MessagingExtensionActionResponse());
    }


    private MessagingExtensionActionResponse TranslateMessage(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action)
    {
        var originalMessage = ((JObject)action.Data).ToObject<TranslationCardResponse>();
        var translationtResult = _translationService.TranslateToEnglishAsync(originalMessage.Message, originalMessage.Language).GetAwaiter().GetResult();
        var translationtResultObject = JsonConvert.DeserializeObject<TranslationModel[]>(translationtResult);

        var adaptiveCardJson = File.ReadAllText(_cards);
        AdaptiveCardTemplate template = new AdaptiveCardTemplate(adaptiveCardJson);

        var myData = new
        {
            FromText = translationtResultObject[0].DetectedLanguage.Language.ToUpper(),
            ToText = translationtResultObject[0].Translations[0].To.ToUpper(),
            OriginalMessage = originalMessage.Message,
            Translation = translationtResultObject[0].Translations[0].Text,
        };

        string cardJson = template.Expand(myData);
        
        var cardAttachment = CreateAdaptiveCardAttachment(cardJson);

        var attachments = new List<MessagingExtensionAttachment>();
        attachments.Add(new MessagingExtensionAttachment
        {
            Content = cardAttachment.Content,
            ContentType = cardAttachment.ContentType,
            Preview = cardAttachment.ToMessagingExtensionAttachment(),
        });

        return new MessagingExtensionActionResponse
        {
            ComposeExtension = new MessagingExtensionResult
            {
                AttachmentLayout = "list",
                Type = "result",
                Attachments = attachments,
            },
        };
    }
    private static Attachment CreateAdaptiveCardAttachment(string _card)
    {
        var adaptiveCardAttachment = new Attachment()
        {
            ContentType = "application/vnd.microsoft.card.adaptive",
            Content = JsonConvert.DeserializeObject(_card),
        };
        return adaptiveCardAttachment;
    }

    public class TranslationCardResponse
    {
        public string Message { get; set; }
        public string Language { get; set; }
    }

}

