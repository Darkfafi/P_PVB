using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ramses.Confactory;
using System;
using Ramses.SceneTrackers;

public enum GamePhase
{
    None,
    Lobby,
    Factions,
    Skills,
    Turns
}

public class ConCurrentPhase : IConfactory
{
    public GamePhase CurrentGamePhase { get; private set; }

    private PhasesTranslator _phasesTranslator;
    private ConPlayers _conPlayers;

    public ConCurrentPhase()
    {
        _conPlayers = ConfactoryFinder.Instance.Get<ConPlayers>();
        _phasesTranslator = SceneTrackersFinder.Instance.GetSceneTracker<AirConsoleMessageST>().Get<PhasesTranslator>();
        CurrentGamePhase = GamePhase.None;

        if(_conPlayers.IsReadyToUse)
        {
            OnReady();
        }
        else
        {
            _conPlayers.ConPlayerReadyToUseEvent += OnConPlayerReadyToUseEvent;
        }
    }

    private void OnConPlayerReadyToUseEvent()
    {
        _conPlayers.ConPlayerReadyToUseEvent -= OnConPlayerReadyToUseEvent;
        OnReady();
    }

    public void SetCurrentPhase(GamePhase gamePhase)
    {
        CurrentGamePhase = gamePhase;
        if (!_conPlayers.IsReadyToUse) { return; } // It will be send to the players on the ready event.
        RegisteredPlayer[] allPlayersConnected = _conPlayers.GetCurrentlyRegisteredPlayers(true);
        for(int i = 0; i < allPlayersConnected.Length; i++)
        {
            UpdatePhaseInfoForPlayer(allPlayersConnected[i]);
        }
    }

    public void ConClear()
    {
        _conPlayers.ConPlayerReadyToUseEvent -= OnConPlayerReadyToUseEvent;
        _conPlayers.PlayerRegisteredEvent -= UpdatePhaseInfoForPlayer;
        _conPlayers.RegisteredPlayerReconnectedEvent -= UpdatePhaseInfoForPlayer;
    }

    private void OnReady()
    {
        // Start Logics
        _conPlayers.PlayerRegisteredEvent += UpdatePhaseInfoForPlayer;
        _conPlayers.RegisteredPlayerReconnectedEvent += UpdatePhaseInfoForPlayer;
        SetCurrentPhase(CurrentGamePhase);
    }

    private void UpdatePhaseInfoForPlayer(RegisteredPlayer player)
    {
        _phasesTranslator.UpdateOnCurrentPhase(CurrentGamePhase, player.DeviceID);
    }
}
