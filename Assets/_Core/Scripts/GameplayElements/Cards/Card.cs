using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ramses.SceneTrackers;
using System;

public abstract class BaseCard
{
    // TODO: This will not have references to any GameObjects. This class is only data. And the UI represents what happens directed by events.
    public string CardName { get; private set; }
    public abstract CardType CardType { get; }

    public BaseCard(string cardName)
    {
        CardName = cardName;
        Debug.Log(CardName);
    }

    public bool IsPlayable(FactionType factionPlayingCard)
    {
        return IsTypeCardPlayable(factionPlayingCard);
    }

    protected abstract bool IsTypeCardPlayable(FactionType factionPlayingCard);
}

public class Card : BaseCard
{
    public override CardType CardType
    {
        get
        {
            return CardType.BaseCard;
        }
    }

    public string CardName { get; private set; }
    public CardDefinitionBaseItem CardDefinitionItem { get; private set; }

    public Card(string cardName, CardDefinitionBaseItem ownCardDefinitionItem) : base(cardName)
    {
        CardName = cardName;
        CardDefinitionItem = ownCardDefinitionItem;
    }

    protected override bool IsTypeCardPlayable(FactionType factionPlayingCard)
    {
        return SceneTrackersFinder.Instance.GetSceneTracker<PlayfieldST>().Playfield.GetCornerByFaction(factionPlayingCard).CanBuildStructureForCard(this);
    }
}

public class UpgradeCard : BaseCard
{
    public override CardType CardType
    {
        get
        {
            return CardType.UpgradeCard;
        }
    }

    public CardDefinitionUpgradeItem CardDefinitionItem { get; private set; }
    public CardDefinitionBaseItem BaseCardDefinitionItem { get; private set; }

    public UpgradeCard(string cardName, CardDefinitionUpgradeItem ownCardDefinitionItem, CardDefinitionBaseItem baseCardDefinitionItem) : base(cardName)
    {
        CardDefinitionItem = ownCardDefinitionItem;
        this.BaseCardDefinitionItem = baseCardDefinitionItem;
    }

    protected override bool IsTypeCardPlayable(FactionType factionPlayingCard)
    {
        PlayerCorner corner = SceneTrackersFinder.Instance.GetSceneTracker<PlayfieldST>().Playfield.GetCornerByFaction(factionPlayingCard);
        return corner.CanBuildStructureForCard(this);
    }
}