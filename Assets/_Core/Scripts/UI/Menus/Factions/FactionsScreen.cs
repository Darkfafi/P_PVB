using UnityEngine;
using Ramses.Confactory;
using Ramses.SceneTrackers;
using System;

public class FactionsScreen : MonoBehaviour
{
    [SerializeField]
    private FactionSelect[] _factionSelectTabs;

    private ConPlayers _conPlayers;
    private ConPlayerFactions _conPlayerFactions;
    private FactionsTranslator _factionsTranslator;

    protected void Awake()
    {
        _conPlayers = ConfactoryFinder.Instance.Get<ConPlayers>();
        _conPlayerFactions = ConfactoryFinder.Instance.Get<ConPlayerFactions>();
        _factionsTranslator = SceneTrackersFinder.Instance.GetSceneTracker<AirConsoleMessageST>().Get<FactionsTranslator>();
        if(_conPlayers.IsReadyToUse)
        {
            OnReady();
        }
        else
        {
            _conPlayers.ConPlayerReadyToUseEvent += OnConPlayerReadyToUseEvent;
        }
    }

    protected void OnDestroy()
    {
        _conPlayers.ConPlayerReadyToUseEvent -= OnConPlayerReadyToUseEvent;
        UnlistenToDevices();
    }

    private void ListenToDevices()
    {
        UnlistenToDevices();
        _conPlayers.RegisteredPlayerReconnectedEvent += OnRegisteredPlayerReconnectedEvent;
        _factionsTranslator.FactionRequestEvent += OnFactionRequestEvent;
    }

    private void OnRegisteredPlayerReconnectedEvent(RegisteredPlayer player)
    {
        SendUpdateFactions();
    }

    private void UnlistenToDevices()
    {
        _factionsTranslator.FactionRequestEvent -= OnFactionRequestEvent;
        _conPlayers.RegisteredPlayerReconnectedEvent -= OnRegisteredPlayerReconnectedEvent;
    }

    private void OnReady()
    {
        ListenToDevices();
//        DEBUG_AssignFactionsToPlayersAndContinue();
    }

    private void OnFactionRequestEvent(int deviceId, FactionType factionType, bool selectType)
    {
        PlayerFactionLinkItem ft = _conPlayerFactions.GetLinkItemForFaction(factionType);
        RegisteredPlayer playerOfId = _conPlayers.GetRegisteredPlayerById(deviceId);
        if (selectType)
        {
            if (ft.Player != null) { return; }
            _conPlayerFactions.AssignPlayerToFaction(playerOfId, factionType);
        }
        else
        {
            if (ft.Player != playerOfId) { return; }
            _conPlayerFactions.UnassignPlayerFromItsFaction(playerOfId);
        }

        SendUpdateFactions();
    }

    private void SendUpdateFactions()
    {
        FactionType[] freeFactions = _conPlayerFactions.GetFreeFactions();
        int[] ffis = new int[freeFactions.Length];
        for(int i = 0; i < freeFactions.Length; i++)
        {
            int factionIndex = (int)freeFactions[i];
            ffis[i] = factionIndex;
        }

        _factionsTranslator.SendUpdateFactionsAvailable(ffis);
    }

    private void DEBUG_AssignFactionsToPlayersAndContinue()
    {
        RegisteredPlayer[] players = ConfactoryFinder.Instance.Get<ConPlayers>().GetCurrentlyRegisteredPlayers(false);
        FactionType[] types = _conPlayerFactions.GetFreeFactions();
        FactionType t = FactionType.Samurai;
        for(int i = 0; i <  players.Length; i++)
        {
            if (i == 0)
                t = FactionType.Knights;
            if (i == 1)
                t = FactionType.Samurai;
            if (i == 2)
                t = FactionType.Vikings;
            if (i == 3)
                t = FactionType.Spartans;
            _conPlayerFactions.AssignPlayerToFaction(players[i], t);
        }
        Invoke("GoToGameScene", 1f);
        Debug.LogError("DEBUG METHOD CALLED!: DEBUG_AssignFactionsToPlayersAndContinue", this.gameObject);
    }

    private void GoToGameScene()
    {
        ConfactoryFinder.Instance.Get<ConSceneSwitcher>().SwitchScreen(SceneNames.GAME_SCENE);
    }

    private void OnConPlayerReadyToUseEvent()
    {
        _conPlayers.ConPlayerReadyToUseEvent -= OnConPlayerReadyToUseEvent;
        OnReady();
    }
}
