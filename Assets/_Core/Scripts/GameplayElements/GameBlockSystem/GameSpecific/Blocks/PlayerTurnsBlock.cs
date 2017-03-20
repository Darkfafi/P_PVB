using System;
using UnityEngine;
using Ramses.SceneTrackers;

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
    private ConPlayers _conPlayer;
    private TurnSystem _turnSystem = new TurnSystem(false);

    private SkillEffects _skillEffects;

    // Translators
    private CardInteractionTranslator _cardInteractionTranslator;
    private CoinTranslator _coinTranslator;
    private SkillTranslator _skillTranslator;

    protected override void Initialized()
    {
        _skillEffects = new SkillEffects(game);
        _cardInteractionTranslator = SceneTrackersFinder.Instance.GetSceneTracker<AirConsoleMessageST>().Get<CardInteractionTranslator>();
        _coinTranslator = SceneTrackersFinder.Instance.GetSceneTracker<AirConsoleMessageST>().Get<CoinTranslator>();
        _skillTranslator = SceneTrackersFinder.Instance.GetSceneTracker<AirConsoleMessageST>().Get<SkillTranslator>();

        _conPlayer = Ramses.Confactory.ConfactoryFinder.Instance.Get<ConPlayers>();

        for (int i = 0; i < game.GamePlayers.Length; i++)
        {
            game.GamePlayers[i].ReceivedCardEvent += OnReceivedCardEvent;
        }
    }

    protected override void Activated()
    {
        Ramses.Confactory.ConfactoryFinder.Instance.Get<ConCurrentPhase>().SetCurrentPhase(GamePhase.Turns);

        _cardInteractionTranslator.CardPlayRequestEvent += OnCardPlayRequestEvent;
        _cardInteractionTranslator.DrawCardsRequestEvent += OnDrawCardsRequestEvent;

        _coinTranslator.CoinRequestEvent += OnCoinRequestEvent;

        _skillTranslator.SkillUseRequestEvent += OnSkillUseRequestEvent;

        _conPlayer.RegisteredPlayerDisconnectedEvent += OnRegisteredPlayerDisconnectedEvent;

        _turnSystem.TurnStartedEvent += OnTurnStartedEvent;
        _turnSystem.TurnSystemEndedEvent += OnTurnSysemEndedEvent;

        _skillEffects.SkillEffectDoneEvent += OnSkillEffectDoneEvent;

        for (int i = 0; i < game.GamePlayers.Length; i++)
        {
            game.GamePlayers[i].PlayCardEvent += OnPlayCardEvent;
            game.GamePlayers[i].SkillPouch.SkillUsedEvent += OnSkillUsedEvent;
            _turnSystem.AddTurnTickets(game.GamePlayers[i].PlayerIndex); // Player index used as id because the index does not change on device change.
            _turnSystem.SetPriorityLevelOfTicket(game.GamePlayers[i].PlayerIndex, Ramses.Confactory.ConfactoryFinder.Instance.Get<ConSkills>().GetIndexValueOfSkill(game.GamePlayers[i].SkillPouch.Skill));
        }

        _turnSystem.StartTurnSystem();

        Debug.Log("PlayerTurnsBlock Activated");
    }

    protected override void CycleEnded()
    {

    }

    protected override void CycleStarted()
    {
        Debug.Log("Started PlayerTurnsBlock " + gameBlockInfo.SecondsForEachPlayerTurn);
    }

    protected override void Deactivated()
    {
        _cardInteractionTranslator.CardPlayRequestEvent -= OnCardPlayRequestEvent;
        _cardInteractionTranslator.DrawCardsRequestEvent -= OnDrawCardsRequestEvent;

        _coinTranslator.CoinRequestEvent -= OnCoinRequestEvent;

        _skillTranslator.SkillUseRequestEvent -= OnSkillUseRequestEvent;

        _conPlayer.RegisteredPlayerDisconnectedEvent -= OnRegisteredPlayerDisconnectedEvent;

        _turnSystem.TurnStartedEvent -= OnTurnStartedEvent;
        _turnSystem.TurnSystemEndedEvent -= OnTurnSysemEndedEvent;

        _turnSystem.ClearTurnTickets();

        _skillEffects.SkillEffectDoneEvent -= OnSkillEffectDoneEvent;

        for (int i = 0; i < game.GamePlayers.Length; i++)
        {
            game.GamePlayers[i].PlayCardEvent -= OnPlayCardEvent;
            game.GamePlayers[i].SkillPouch.SkillUsedEvent -= OnSkillUsedEvent;
        }

    }

    protected override void Destroyed()
    {
        for (int i = 0; i < game.GamePlayers.Length; i++)
        {
            game.GamePlayers[i].ReceivedCardEvent -= OnReceivedCardEvent;
        }
    }

    private void OnRegisteredPlayerDisconnectedEvent(RegisteredPlayer player)
    {
        if (_turnSystem.CurrentTurnTicket == player.PlayerIndex)
            _turnSystem.EndTurnForCurrentTicket();
    }

    private void OnTurnSysemEndedEvent()
    {
        NextBlock();
    }

    private void OnTurnStartedEvent(int gamePlayerIndex)
    {
        if(!game.GetGamePlayerBy(gamePlayerIndex).IsConnected)
        {
            _turnSystem.EndTurnForCurrentTicket();
        }
        Debug.Log(game.GetGamePlayerBy(gamePlayerIndex).FactionType + " <-- TURN");
    }
    
    private bool IsPlayerTurn(GamePlayer player)
    {
        Debug.Log(_turnSystem.CurrentTurnTicket + " <- ctt | -> " + player.PlayerIndex);
        return _turnSystem.CurrentTurnTicket == player.PlayerIndex;
    }

    // Requests
    private void OnCardPlayRequestEvent(string cardName, int deviceId)
    {
        GamePlayer p = game.GetGamePlayerByDeviceId(deviceId);
        if (p != null)
        {
            if (!IsPlayerTurn(p)) { return; } // If not this player his turn then ignore
            p.PlayCard(cardName);
        }  
    }

    private void OnDrawCardsRequestEvent(int amount, int deviceId)
    {
        GamePlayer p = game.GetGamePlayerByDeviceId(deviceId);
        if(p != null)
        {
            if (!IsPlayerTurn(p)) { return; }
            if(p.DrawCard(amount))
            {
                _cardInteractionTranslator.SendAllowedDrawRequest(true, "Draw Allowed", p.LinkedPlayer.DeviceID);
                _turnSystem.EndTurnForCurrentTicket();
            }
        }
    }

    private void OnCoinRequestEvent(int amount, int deviceId)
    {
        GamePlayer p = game.GetGamePlayerByDeviceId(deviceId);
        if(p != null)
        {
            if (!IsPlayerTurn(p)) { return; }
            if (p.GrabCoins(amount))
            {
                _turnSystem.EndTurnForCurrentTicket();
            }
        }
    }

    private void OnSkillUseRequestEvent(int deviceId, Skill skill)
    {
        GamePlayer p = game.GetGamePlayerByDeviceId(deviceId);
        if (p == null) { return; }
        if (!IsPlayerTurn(p)) { return; }
        p.SkillPouch.UseSkill();
    }

    // Actions
    private void OnPlayCardEvent(GamePlayer gamePlayer, BaseCard card)
    {
        _cardInteractionTranslator.SendAllowedPlayRequest(true, "Playing Card: " + card.CardName, gamePlayer.LinkedPlayer.DeviceID, card.CardName);
        _cardInteractionTranslator.SendUpdateCardsShown(gamePlayer.LinkedPlayer.DeviceID, gamePlayer.GetNameListOfCardsInHand());
        game.Playfield.GetCornerByFaction(gamePlayer.FactionType).BuildStructureForCard(card);
        _turnSystem.EndTurnForCurrentTicket();
    }

    private void OnReceivedCardEvent(GamePlayer gamePlayer, BaseCard card)
    {
        _cardInteractionTranslator.SendUpdateCardsShown(gamePlayer.LinkedPlayer.DeviceID, gamePlayer.GetNameListOfCardsInHand());
    }

    private void OnSkillUsedEvent(GamePlayer gamePlayer, Skill skill)
    {
        _skillEffects.DoSkillEffect(gamePlayer, skill);
    }

    private void OnSkillEffectDoneEvent(GamePlayer player, Skill skill)
    {
        if (IsPlayerTurn(player))
            _turnSystem.EndTurnForCurrentTicket();
    }
}
