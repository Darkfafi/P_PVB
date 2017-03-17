using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsLibrary : ScriptableObject
{
    [SerializeField]
    private SkillLibraryItem[] _skillLibraryItems;

	public SkillLibraryItem GetSkillItem(Skill skill)
    {
        for(int i = 0; i < _skillLibraryItems.Length; i++)
        {
            if(_skillLibraryItems[i].Skill == skill)
            {
                return _skillLibraryItems[i];
            }
        }
        return null;
    }
}

[Serializable]
public class SkillLibraryItem
{
    public Skill Skill { get { return _skill; } }
    public Sprite SkillIcon { get { return _skillIcon; } }

    [SerializeField]
    private Skill _skill;

    [SerializeField]
    private Sprite _skillIcon;
}