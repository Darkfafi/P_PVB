using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnsBlock : BaseGameBlock<BuildingsGame, PlayerTurnsBlockInfo, PlayerTurnsBlockLogic>
{
    public override PlayerTurnsBlockInfo BlockInfo
    {
        get
        {
            return info;
        }
    }

    [SerializeField]
    private PlayerTurnsBlockInfo info;
}

[Serializable]
public struct PlayerTurnsBlockInfo : IGameBlockInfo<BuildingsGame>
{
    [SerializeField, Range(10, 120)]
    public int SecondsForEachPlayerTurn;
}

public class PlayerTurnsBlockLogic : BaseGameBlockLogic<BuildingsGame, PlayerTurnsBlockInfo>
{
    protected override void Activated()
    {
        Debug.Log("PlayerTurnsBlock Activated");
    }

    protected override void CycleEnded()
    {
        throw new NotImplementedException();
    }

    protected override void CycleStarted()
    {
        Debug.Log("Started PlayerTurnsBlock " + gameBlockInfo.SecondsForEachPlayerTurn);
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
