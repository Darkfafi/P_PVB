using UnityEngine;
using System;
using System.Collections.Generic;

public class CardsDefinitionsLibrary : ScriptableObject
{
    [SerializeField]
    private List<CardDefinitionItem> _cardsDefined;

    public CardDefinitionItem[] GetAllCardDefinitions()
    {
        return _cardsDefined.ToArray();
    }

    public CardDefinitionUpgradeItem[] GetAllCardUpgradeDefinitions()
    {
        List<CardDefinitionUpgradeItem> items = new List<CardDefinitionUpgradeItem>();
        CardDefinitionItem cdi = null;

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

    public bool IsUpgradeCard(string cardName)
    {
        CardDefinitionUpgradeItem[] cduis = GetAllCardUpgradeDefinitions();
        for(int i = 0; i < cduis.Length; i++)
        {
            if (cduis[i].CardName == cardName)
                return true;
        }
        return false;
    }
}

[Serializable]
public class CardDefinitionItem : GlobalCardDefinitionItem
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
    public GameObject CardBuildingObjectPrefab { get { return _buildingObjectPrefab; } }

    [Header("Global Card Settings")]

    [SerializeField]
    private string _cardName;

    [SerializeField]
    private int _cost;

    [SerializeField]
    private Sprite _cardSprite;

    [SerializeField]
    private GameObject _buildingObjectPrefab;
}