using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CardsDefinitionsLibrary : ScriptableObject
{
    [SerializeField]
    private CardDefinitionItem[] _cardsDefined;
}

[Serializable]
public class CardDefinitionItem : GlobalCardDefinitionItem
{
    [Header("Base Card Settings")]

    [SerializeField]
    private Skill _skillLinkedToCard;

    [SerializeField]
    private CardDefinitionUpgradeItem[] _upgrades;
}

[Serializable]
public class CardDefinitionUpgradeItem : GlobalCardDefinitionItem
{
    [Header("Upgrade Card Settings")]
    [SerializeField]
    private int _upgradeCost; 
}

[Serializable]
public class GlobalCardDefinitionItem
{
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