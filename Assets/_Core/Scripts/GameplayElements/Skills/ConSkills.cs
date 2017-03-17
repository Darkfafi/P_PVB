using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ramses.Confactory;
using System;

public class ConSkills : IConfactory
{
    public Skill[] SkillsInOrder { get; private set; }
    public SkillsLibrary SkillLibrary { get; private set; }

    public ConSkills()
    {
        SkillLibrary = Resources.Load<SkillsLibrary>(LibraryLocations.SKILLS_LIBRARY_LOCATION);
        SkillsInOrder = new Skill[]
        {
            Skill.TheCrown,
            Skill.Thief,
            Skill.Miracle,
            Skill.Trade,
            Skill.Destruction
        };

    }

    public int GetIndexValueOfSkill(Skill skill)
    {
        if (skill == Skill.None) { return -1; }
        return SkillsInOrder.GetIndexOf(skill);
    }

    public void ConClear()
    {

    }
}
