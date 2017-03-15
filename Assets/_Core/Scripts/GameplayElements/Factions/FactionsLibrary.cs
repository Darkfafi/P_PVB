using UnityEngine;
using System;
public enum FactionType
{
    None,
    Spartans,
    Vikings,
    Knights,
    Samurai
}

public class FactionsLibrary : ScriptableObject
{
    [SerializeField]
    private FactionLibraryItem[] factionLibraryItems;

    public FactionLibraryItem GetItemByFactionType(FactionType factionType)
    {
        for(int i = 0; i < factionLibraryItems.Length; i++)
        {
            if(factionLibraryItems[i].FactionType == factionType)
            {
                return factionLibraryItems[i];
            }
        }
        Debug.LogError("No FactionLibraryItem for factionType: " + factionType.ToString());
        return null;
    }
}

[Serializable]
public class FactionLibraryItem
{
    public FactionType FactionType { get { return _factionType; } }
    public Sprite FactionPortrait { get { return _factionPortrait; } }
    public Color FactionColor { get { return _factionColor; } }
    public Sprite FactionSectionBackground { get { return _factionSectionBackground; } }

    [SerializeField]
    private FactionType _factionType;

    [SerializeField]
    private Sprite _factionPortrait;

    [SerializeField]
    private Color _factionColor;

    [SerializeField]
    private Sprite _factionSectionBackground;
}
