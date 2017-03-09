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
    public CardsDefinitionsLibrary CardsDefinitionLibrary { get; private set; }

    public ConCards()
    {
        CardsDefinitionLibrary = Resources.Load<CardsDefinitionsLibrary>(LibraryLocations.CARDS_DEFINITION_LIBRARY_LOCATION);
    }

    public void ConClear()
    {

    }

    public Card CreateCard(string cardName, CardType cardType)
    {
        Card cardCreating = null;
        // TODO: insert information of cardCreating. If the cardName != a defined card. Then return null.
        return cardCreating;
    }
}
