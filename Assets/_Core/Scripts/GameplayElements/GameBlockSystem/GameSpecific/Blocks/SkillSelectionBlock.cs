using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSelectionBlock : BaseGameBlock<BuildingsGame, SkillSelectionBlockInfo, SkillSelectionBlockLogics>
{
    public override SkillSelectionBlockInfo BlockInfo
    {
        get
        {
            return info;
        }
    }

    [SerializeField]
    private SkillSelectionBlockInfo info;
}

[Serializable]
public struct SkillSelectionBlockInfo : IGameBlockInfo<BuildingsGame>
{
    [SerializeField, Range(10, 120)]
    public int SecondsForEachSkillSelectionTurn;
}

public class SkillSelectionBlockLogics : BaseGameBlockLogic<BuildingsGame, SkillSelectionBlockInfo>
{
    protected override void Activated()
    {
        Debug.Log("Activated Skill: " + gameBlockInfo.SecondsForEachSkillSelectionTurn);
    }

    protected override void CycleEnded()
    {
        throw new NotImplementedException();
    }

    protected override void CycleStarted()
    {
        Debug.Log("Started Skill: " + gameBlockInfo.SecondsForEachSkillSelectionTurn);
    }

    protected override void Deactivated()
    {
        throw new NotImplementedException();
    }

    protected override void Destroyed()
    {
        throw new NotImplementedException();
    }
}
