using UnityEngine;
using System;
using System.Collections.Generic;

public class CardsDefinitionsLibrary : ScriptableObject
{
    [SerializeField]
    private List<CardDefinitionBaseItem> _cardsDefined;

    public CardDefinitionBaseItem[] GetAllBaseCardDefinitions()
    {
        return _cardsDefined.ToArray();
    }

    public GlobalCardDefinitionItem[] GetAllCardDefinitions()
    {
        List<GlobalCardDefinitionItem> globalCards = new List<GlobalCardDefinitionItem>(_cardsDefined.ToArray());
        globalCards.AddRange(GetAllCardUpgradeDefinitions());
        return globalCards.ToArray();
    }

    public GlobalCardDefinitionItem GetCardDefinitionByName(string cardName)
    {
        GlobalCardDefinitionItem[] cdis = GetAllCardDefinitions();

        for(int i = 0; i < cdis.Length; i++)
        {
            if (cdis[i].CardName == cardName)
                return cdis[i];
        }
        Debug.LogWarning("NO CARD FOUND WITH THE NAME: " + cardName);
        return null;
    }

    public CardDefinitionUpgradeItem[] GetAllCardUpgradeDefinitions()
    {
        List<CardDefinitionUpgradeItem> items = new List<CardDefinitionUpgradeItem>();
        CardDefinitionBaseItem cdi = null;

        for (int i = 0; i < _cardsDefined.Count; i++)
        {
            cdi = _cardsDefined[i];
            for(int j = 0; j < cdi.BaseCardUpgrades.Length; j++)
            {
                items.Add(cdi.BaseCardUpgrades[i]);
            }
        }

        return items.ToArray();
    }

    public CardDefinitionBaseItem GetCardBaseItemOfUpgradeItem(CardDefinitionUpgradeItem upgradeItem)
    {
        CardDefinitionBaseItem[] items = GetAllBaseCardDefinitions();
        for(int i = 0; i < items.Length; i++)
        {
            for(int j = 0; j < items[i].BaseCardUpgrades.Length; j++)
            {
                if (items[i].BaseCardUpgrades[j] == upgradeItem)
                    return items[i];
            }
        }
        return null;
    }

    public CardType GetCardTypeFromCard(string cardName)
    {
        GlobalCardDefinitionItem[] gcd = GetAllCardDefinitions();

        for(int i = 0; i < gcd.Length; i++)
        {
            if(gcd[i].CardName == cardName)
            {
                if(gcd.GetType().IsSubclassOf(typeof(CardDefinitionBaseItem)))
                {
                    return CardType.BaseCard;
                }
                else if(gcd.GetType().IsSubclassOf(typeof(CardDefinitionUpgradeItem)))
                {
                    return CardType.UpgradeCard;
                }
            }
        }

        return CardType.None;
    }
}

[Serializable]
public class CardDefinitionBaseItem : GlobalCardDefinitionItem
{
    public Skill BaseCardSkillLinkedToCard { get { return _skillLinkedToCard; } }
    public CardDefinitionUpgradeItem[] BaseCardUpgrades { get { return _upgrades; } }

    [Header("Base Card Settings")]
    [SerializeField]
    private Skill _skillLinkedToCard;

    [SerializeField]
    private CardDefinitionUpgradeItem[] _upgrades;
}

[Serializable]
public class CardDefinitionUpgradeItem : GlobalCardDefinitionItem
{
    public int UpgradeCardUpgradeCost { get { return _upgradeCost; } }

    [Header("Upgrade Card Settings")]
    [SerializeField]
    private int _upgradeCost; 
}

[Serializable]
public class GlobalCardDefinitionItem
{
    public string CardName { get { return _cardName; } }
    public int CardCost { get { return _cost; } }
    public Sprite CardSprite { get { return _cardSprite; } }
    public Building CardBuildingObjectPrefab { get { return _buildingObjectPrefab; } }

    [Header("Global Card Settings")]

    [SerializeField]
    private string _cardName;

    [SerializeField]
    private int _cost;

    [SerializeField]
    private Sprite _cardSprite;

    [SerializeField]
    private Building _buildingObjectPrefab;
}