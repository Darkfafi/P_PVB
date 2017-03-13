﻿using Newtonsoft.Json.Linq;
using UnityEngine;
using NDream.AirConsole;

public delegate void CardNameDeviceHandler(string cardName, int deviceId);
public delegate void AmountDeviceHandler(int amount, int deviceId);

public class CardInteractionTranslator : BaseACMessageTranslator
{
    /// <summary>
    /// Is triggered when a device requests for playing a card. 
    /// To show the screen has received the message the method 'SendAllowedPlayRequest' must be called.
    /// </summary>
    public event CardNameDeviceHandler CardPlayRequestEvent;
    public event AmountDeviceHandler DrawCardsRequestEvent;

    // Allow calls

    /// <summary>
    /// This method is a responce to the 'CardPlayRequestEvent'
    /// If it was allowed, the message action will contain "playCardRequestAllowed" else "playCardRequestNotAllowed"
    /// </summary>
    /// <param name="isAllowed">Indicates if the request was allowed</param>
    /// <param name="deviceId">The device which is responded to</param>
    /// <param name="allowedCardName">The card of the spoken subject</param>
    /// <param name="allowMessage">Extra information which is send to the device. This allowes for error messages if so desired</param>
    public void SendAllowedPlayRequest(bool isAllowed, string allowMessage, int deviceId, string allowedCardName)
    {
        var message = new
        {
            action = (isAllowed) ? "playCardRequestAllowed" : "playCardRequestNotAllowed",
            info = new { cardName = allowedCardName, allowMessage = allowMessage}
        };

        AirConsole.instance.Message(deviceId, message);
    }

    public void SendAllowedDrawRequest(bool isAllowed, string allowMessage, int deviceId, params string[] drawedCards)
    {
        string bindedString = CreateParsableString(drawedCards);
        var message = new
        {
            action = (isAllowed) ? "drawCardsRequestAllowed" : "drawCardsRequestNotAllowed",
            info = new { cardNames = bindedString, allowMessage = allowMessage }
        };

        AirConsole.instance.Message(deviceId, message);
    }

    // Global Calls

    /// <summary>
    /// Notifies the controller which cards to display.
    /// This method must be called after all changes to the cardhand. This means: Playing cards, drawing cards, discarding cards etc..
    /// The cards to display will be received in the info, cardNames and cardLocations. All parseable by ',' and in correct order.
    /// </summary>
    /// <param name="cardsToShow">The cards to show on the controller</param>
    public void SendUpdateCardsShown(int deviceId, params string[] cardsToShow)
    {
        string cardNamesFromLib = "";
        string cardLocationsFromLib = "";

        for (int i = 0; i < cardsToShow.Length; i++)
        {
            cardNamesFromLib += cardsToShow[i];
            cardLocationsFromLib += ConCards.CARDS_IMAGE_LOCATION + Ramses.Confactory.ConfactoryFinder.Instance.Get<ConCards>().CardsDefinitionLibrary.GetCardDefinitionByName(cardsToShow[i]).CardSprite.name;
            if (i < cardsToShow.Length - 1)
            {
                cardNamesFromLib += ",";
                cardLocationsFromLib += ",";
            }
        }

        var message = new
        {
            action = "UpdateCardsDisplay",
            info = new { cardNames = cardNamesFromLib, cardLocations = cardLocationsFromLib }
        };

        AirConsole.instance.Message(deviceId, message);
    }

    protected override void MessageReceived(int from, JToken data)
    {
        if (SendEventIfPlayRequest(from, data)) { return; }
        if (SendEventIfDrawRequest(from, data)) { return; }
    }

    // Checks / Event Senders

    private bool SendEventIfDrawRequest(int from, JToken data)
    {
        if(data["drawCardsRequest"] != null)
        {
            if(data[AirConsoleMessageST.INFO_PARAMETER] != null && data[AirConsoleMessageST.INFO_PARAMETER]["cardAmount"] != null)
            {
                if(DrawCardsRequestEvent != null)
                {
                    DrawCardsRequestEvent((int)data[AirConsoleMessageST.INFO_PARAMETER]["cardAmount"], from);
                    return true;
                }
            }
        }
        return false;
    }

    private bool SendEventIfPlayRequest(int from, JToken data)
    {
        if (data["playCardRequest"] != null)
        {
            if (data[AirConsoleMessageST.INFO_PARAMETER] != null && data[AirConsoleMessageST.INFO_PARAMETER]["cardName"] != null)
            {
                if (CardPlayRequestEvent != null)
                {
                    CardPlayRequestEvent((string)data[AirConsoleMessageST.INFO_PARAMETER]["cardName"], from);
                    return true;
                }
            }
            else
            {
                Debug.LogError("No info is found in the 'playCardRequest'");
            }
        }
        return false;
    }

    // Tools

    private string CreateParsableString(params string[] separateStringsToBind)
    {
        string binds = "";

        for (int i = 0; i < separateStringsToBind.Length; i++)
        {
            binds += separateStringsToBind[i];
            if (i < separateStringsToBind.Length - 1)
            {
                binds += ",";
            }
        }
        return binds;
    }
}
