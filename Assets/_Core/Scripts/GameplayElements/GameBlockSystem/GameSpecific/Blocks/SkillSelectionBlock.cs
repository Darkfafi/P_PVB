using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ramses.SceneTrackers;
using Ramses.Confactory;

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
    private bool _firstAssignmentInGame = false;
    private TurnSystem _turnSystem = new TurnSystem(false);
    private SkillTranslator _skillTranslator;

    protected override void Initialized()
    {
        _skillTranslator = SceneTrackersFinder.Instance.GetSceneTracker<AirConsoleMessageST>().Get<SkillTranslator>();

        _skillTranslator.SkillPickRequestEvent += OnSkillPickRequestEvent;
    }

    protected override void Activated()
    {
        ConfactoryFinder.Instance.Get<ConCurrentPhase>().SetCurrentPhase(GamePhase.Skills);
        Debug.Log("Activated Skill: " + gameBlockInfo.SecondsForEachSkillSelectionTurn);

        for(int i = 0; i < game.GamePlayers.Length; i++)
        {
            game.GamePlayers[i].SkillPouch.SkillSetEvent += OnSkillSetEvent;
        }
        //NextBlock(); // TODO: Make block functionality and run it before NextBlock call.
    }

    protected override void CycleEnded()
    {

    }

    protected override void CycleStarted()
    {
        _firstAssignmentInGame = true;
        Debug.Log("Started Skill: " + gameBlockInfo.SecondsForEachSkillSelectionTurn);
    }

    protected override void Deactivated()
    {
        for (int i = 0; i < game.GamePlayers.Length; i++)
        {
            game.GamePlayers[i].SkillPouch.SkillSetEvent -= OnSkillSetEvent;
        }
    }

    protected override void Destroyed()
    {
        _skillTranslator.SkillPickRequestEvent -= OnSkillPickRequestEvent;
    }

    // Actions
    private void OnSkillSetEvent(GamePlayer gamePlayer, Skill skill)
    {
        List<Skill> availableSkills = new List<Skill>(ConfactoryFinder.Instance.Get<ConSkills>().SkillsInOrder);
        for(int i = availableSkills.Count - 1; i >= 0; i--)
        {
            for(int j = 0; j <= game.GamePlayers.Length; j++)
            {
                if (availableSkills.Contains(game.GamePlayers[j].SkillPouch.Skill))
                    availableSkills.Remove(game.GamePlayers[j].SkillPouch.Skill);
            }
        }
        _skillTranslator.UpdateSkillsAvailable(availableSkills.ToArray());
    }

    // Requirements
    private void OnSkillPickRequestEvent(int deviceId, Skill skill)
    {

    }
}
