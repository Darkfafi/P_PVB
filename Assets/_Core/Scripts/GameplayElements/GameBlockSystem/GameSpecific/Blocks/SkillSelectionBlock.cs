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

    private Skill _skillPutAsside = Skill.None;

    protected override void Initialized()
    {
        _skillTranslator = SceneTrackersFinder.Instance.GetSceneTracker<AirConsoleMessageST>().Get<SkillTranslator>();

        _skillTranslator.SkillPickRequestEvent += OnSkillPickRequestEvent;
    }

    protected override void Activated()
    {
        ConfactoryFinder.Instance.Get<ConCurrentPhase>().SetCurrentPhase(GamePhase.Skills);
        Debug.Log("Activated Skill: " + gameBlockInfo.SecondsForEachSkillSelectionTurn);

        _turnSystem.TurnStartedEvent += OnTurnStartedEvent;
        _turnSystem.TurnSystemEndedEvent += OnTurnSysemEndedEvent;

        PutSkillAsside(); // Puts random skill aside so the last player can choose from that skill or his own.

        for (int i = 0; i < game.GamePlayers.Length; i++)
        {
            game.GamePlayers[i].SkillPouch.SetSkill(Skill.None);
            game.GamePlayers[i].SkillPouch.SkillSetEvent += OnSkillSetEvent;
            _turnSystem.AddTurnTickets(game.GamePlayers[i].PlayerIndex);
            // TODO: Set priority level on who choses skills first.
        }

        _turnSystem.StartTurnSystem();
    }

    private void PutSkillAsside()
    {
        _skillPutAsside = Skill.None;
        Skill[] availableSkills = GetSkillsAvailable();
        _skillPutAsside = availableSkills[UnityEngine.Random.Range(0, availableSkills.Length)];
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

        if(_turnSystem.CurrentTicketDistanceFromLastTicket() == 0)
        {
            _skillPutAsside = Skill.None; // The skill put asside will be available for the last player chosing a skill
        }

        _skillTranslator.UpdateSkillsAvailable(game.GetGamePlayerBy(gamePlayerIndex).LinkedPlayer.DeviceID, GetSkillsAvailable());
        Debug.Log(game.GetGamePlayerBy(gamePlayerIndex).FactionType + " <-- TURN");
    }

    private void OnTurnSysemEndedEvent()
    {
        NextBlock();
    }

    private Skill[] GetSkillsAvailable(params Skill[] lockedSkills)
    {
        List<Skill> availableSkills = new List<Skill>(ConfactoryFinder.Instance.Get<ConSkills>().SkillsInOrder);

        for(int i = 0; i < lockedSkills.Length; i++)
        {
            if (availableSkills.Contains(lockedSkills[i]))
                availableSkills.Remove(lockedSkills[i]);
        }

        if (availableSkills.Contains(_skillPutAsside))
            availableSkills.Remove(_skillPutAsside);

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
