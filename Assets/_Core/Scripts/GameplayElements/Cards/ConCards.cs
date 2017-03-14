using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ramses.Confactory;
using System;

public enum CardType
{
    None,
    BaseCard,
    UpgradeCard
}

public class ConCards : IConfactory
{
    public const string CARDS_IMAGE_LOCATION = "cards/";

    public CardsDefinitionsLibrary CardsDefinitionLibrary { get; private set; }

    public ConCards()
    {
        CardsDefinitionLibrary = Resources.Load<CardsDefinitionsLibrary>(LibraryLocations.CARDS_DEFINITION_LIBRARY_LOCATION);
    }

    public void ConClear()
    {

    }

    public BaseCard CreateCard(string cardName)
    {
        BaseCard cardCreating = null;
        GlobalCardDefinitionItem cardRepresentingItem = CardsDefinitionLibrary.GetCardDefinitionByName(cardName);

        if (cardRepresentingItem != null)
        {
            if(cardRepresentingItem.GetType().IsSubclassOf(typeof(CardDefinitionBaseItem)))
            {
                cardCreating = new Card(cardName, (CardDefinitionBaseItem)cardRepresentingItem);
            }
            else if(cardRepresentingItem.GetType().IsSubclassOf(typeof(CardDefinitionUpgradeItem)))
            {
                cardCreating = new UpgradeCard(cardName, (CardDefinitionUpgradeItem)cardRepresentingItem, CardsDefinitionLibrary.GetCardBaseItemOfUpgradeItem((CardDefinitionUpgradeItem)cardRepresentingItem));
            }
        }

        return cardCreating;
    }
}
