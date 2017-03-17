using UnityEngine;
using DG.Tweening;
using System;

public class CardPile : BaseFieldPile<BaseCard>
{
    protected override BaseCard ObjectGrabbing()
    {
        BaseCard card;
        ConCards cc = Ramses.Confactory.ConfactoryFinder.Instance.Get<ConCards>();
        GlobalCardDefinitionItem[] cardItems = cc.CardsDefinitionLibrary.GetAllCardDefinitions();
        card = cc.CreateCard(cardItems[UnityEngine.Random.Range(0, cardItems.Length)].CardName);
        return card;
    }
}