using System;
using UnityEngine;
using Ramses.SceneTrackers;
using System.Collections.Generic;

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
    private CardInteractionTranslator _cardInteractionTranslator;
    private TurnSystem _turnSystem = new TurnSystem(false);

    protected override void Initialized()
    {
        _cardInteractionTranslator = SceneTrackersFinder.Instance.GetSceneTracker<AirConsoleMessageST>().Get<CardInteractionTranslator>();

        for (int i = 0; i < game.GamePlayers.Length; i++)
        {
            game.GamePlayers[i].ReceivedCardEvent += OnReceivedCardEvent;
        }
    }

    protected override void Activated()
    {
        _cardInteractionTranslator.CardPlayRequestEvent += OnCardPlayRequestEvent;
        _cardInteractionTranslator.DrawCardsRequestEvent += OnDrawCardsRequestEvent;

        _turnSystem.TurnStartedEvent += OnTurnStartedEvent;
        _turnSystem.TurnSystemEndedEvent += OnTurnSysemEndedEvent;

        for (int i = 0; i < game.GamePlayers.Length; i++)
        {
            game.GamePlayers[i].PlayCardEvent += OnPlayCardEvent;
            _turnSystem.AddTurnTickets(game.GamePlayers[i].PlayerIndex); // Player index used as id because the index does not change on device change.
            //_turnSystem.SetPriorityLevelOfTicket(game.GamePlayers[i].PlayerIndex, ) // TODO: Set priority to skill turn count.
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

        _turnSystem.TurnStartedEvent -= OnTurnStartedEvent;
        _turnSystem.TurnSystemEndedEvent -= OnTurnSysemEndedEvent;

        _turnSystem.ClearTurnTickets();

        for (int i = 0; i < game.GamePlayers.Length; i++)
        {
            game.GamePlayers[i].PlayCardEvent -= OnPlayCardEvent;
        }

    }

    protected override void Destroyed()
    {
        for (int i = 0; i < game.GamePlayers.Length; i++)
        {
            game.GamePlayers[i].ReceivedCardEvent -= OnReceivedCardEvent;
        }
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
        _cardInteractionTranslator.SendUpdateCardsShown(gamePlayer.LinkedPlayer.DeviceID, gamePlayer.CardsInHand);
    }
}
