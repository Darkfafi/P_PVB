using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSelectionBlock : BaseGameBlock<BuildingsGame, SkillSelectionBlockInfo, SkillSelectionBlockLogic>
{
    public override SkillSelectionBlockInfo BlockInfo
    {
        get
        {
            return _info;
        }
    }

    [SerializeField]
    private SkillSelectionBlockInfo _info;
}

[Serializable]
public struct SkillSelectionBlockInfo : IGameBlockInfo<BuildingsGame>
{
    [SerializeField, Range(10, 120)]
    public int SecondsForEachSkillSelectionTurn;
}

public class SkillSelectionBlockLogic : BaseGameBlockLogic<BuildingsGame, SkillSelectionBlockInfo>
{
    protected override void Initialized()
    {

    }

    protected override void Activated()
    {
        Debug.Log("Activated Skill: " + gameBlockInfo.SecondsForEachSkillSelectionTurn);
        NextBlock(); // TODO: Make block functionality and run it before NextBlock call.
    }

    protected override void CycleEnded()
    {

    }

    protected override void CycleStarted()
    {
        Debug.Log("Started Skill: " + gameBlockInfo.SecondsForEachSkillSelectionTurn);
    }

    protected override void Deactivated()
    {

    }

    protected override void Destroyed()
    {

    }
}
