using System;
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
        NextBlock(); // TODO: Make block functionality and run it before NextBlock call.
        return;
        ConfactoryFinder.Instance.Get<ConCurrentPhase>().SetCurrentPhase(GamePhase.Skills);
        Debug.Log("Activated Skill: " + gameBlockInfo.SecondsForEachSkillSelectionTurn);

        _turnSystem.TurnStartedEvent += OnTurnStartedEvent;
        _turnSystem.TurnSystemEndedEvent += OnTurnSysemEndedEvent;

        for (int i = 0; i < game.GamePlayers.Length; i++)
        {
            game.GamePlayers[i].SkillPouch.SkillSetEvent += OnSkillSetEvent;
            _turnSystem.AddTurnTickets(game.GamePlayers[i].PlayerIndex);
            // TODO: Set priority level on who choses skills first.
        }

        _turnSystem.StartTurnSystem();
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
        _turnSystem.TurnStartedEvent -= OnTurnStartedEvent;
        _turnSystem.TurnSystemEndedEvent -= OnTurnSysemEndedEvent;
        _turnSystem.ClearTurnTickets();

        for (int i = 0; i < game.GamePlayers.Length; i++)
        {
            game.GamePlayers[i].SkillPouch.SkillSetEvent -= OnSkillSetEvent;
        }
    }

    protected override void Destroyed()
    {
        _skillTranslator.SkillPickRequestEvent -= OnSkillPickRequestEvent;
    }

    private void OnTurnStartedEvent(int gamePlayerIndex)
    {
        if (!game.GetGamePlayerBy(gamePlayerIndex).IsConnected)
        {
            _turnSystem.EndTurnForCurrentTicket();
        }
        _skillTranslator.UpdateSkillsAvailable(game.GetGamePlayerBy(gamePlayerIndex).LinkedPlayer.DeviceID, GetSkillsAvailable());
        Debug.Log(game.GetGamePlayerBy(gamePlayerIndex).FactionType + " <-- TURN");
    }

    private void OnTurnSysemEndedEvent()
    {
        NextBlock();
    }

    private Skill[] GetSkillsAvailable()
    {
        List<Skill> availableSkills = new List<Skill>(ConfactoryFinder.Instance.Get<ConSkills>().SkillsInOrder);
        for (int i = availableSkills.Count - 1; i >= 0; i--)
        {
            for (int j = 0; j < game.GamePlayers.Length; j++)
            {
                if (availableSkills.Contains(game.GamePlayers[j].SkillPouch.Skill))
                    availableSkills.Remove(game.GamePlayers[j].SkillPouch.Skill);
            }
        }
        return availableSkills.ToArray();
    }

    // Actions
    private void OnSkillSetEvent(GamePlayer gamePlayer, Skill skill)
    {
        if (!IsPlayerTurn(gamePlayer)) { return; }
        _turnSystem.EndTurnForCurrentTicket();
    }

    // Requirements
    private void OnSkillPickRequestEvent(int deviceId, Skill skill)
    {
        GamePlayer p = game.GetGamePlayerByDeviceId(deviceId);
        if(p != null)
        {
            if (!IsPlayerTurn(p)) { return; }
            p.SkillPouch.SetSkill(skill);
        }
    }

    private bool IsPlayerTurn(GamePlayer player)
    {
        return _turnSystem.CurrentTurnTicket == player.PlayerIndex;
    }
}
