using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ramses.SceneTrackers;

public class BuildingsWinConditionBlock : BaseGameBlock<BuildingsGame, BuildingsWinConditionBlockInfo, BuildingsWinConditionBlockLogic>
{
    public override BuildingsWinConditionBlockInfo BlockInfo
    {
        get
        {
            return _info;
        }
    }

    [SerializeField]
    private BuildingsWinConditionBlockInfo _info;
}

[Serializable]
public struct BuildingsWinConditionBlockInfo : IGameBlockInfo<BuildingsGame>
{
    [SerializeField, Range(1, 8)]
    public int BuildingsNeededToEndGame;
}

public class BuildingsWinConditionBlockLogic : BaseGameBlockLogic<BuildingsGame, BuildingsWinConditionBlockInfo>
{
    private PlayfieldST _playfieldSceneTracker;

    protected override void Initialized()
    {
           
    }

    protected override void Activated()
    {
        Debug.Log("Activated Win Condition");

        if (MaxBuildingsAmountReached())
        {
            GamePlayer winner = game.GetGamePlayerBy(
                Ramses.Confactory.ConfactoryFinder.Instance.Get<ConPlayerFactions>()
                .GetLinkItemForFaction(_playfieldSceneTracker.Playfield.GetFactionWithHeighestScore()).Player);

            game.EndGameWinCondition(winner);
        }
        else
        {
            NextBlock();
        }
    }

    private bool MaxBuildingsAmountReached()
    {
        PlayerCorner[] playerCorners = _playfieldSceneTracker.Playfield.AllPlayCorners; 
        for(int i = 0; i < playerCorners.Length; i++)
        {
            if(playerCorners[i].GetAllBuildFieldsInUse().Length == gameBlockInfo.BuildingsNeededToEndGame)
            {
                return true;
            }
        }
        return false;
    }

    protected override void CycleEnded()
    {

    }

    protected override void CycleStarted()
    {
        Debug.Log("Started Win Condition: " + gameBlockInfo.BuildingsNeededToEndGame);
        game.SpawnBuildGrounds(gameBlockInfo.BuildingsNeededToEndGame);
    }

    protected override void Deactivated()
    {

    }

    protected override void Destroyed()
    {

    }
}
