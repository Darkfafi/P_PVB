using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ramses.Confactory;
using System;

public class FactionsScreen : MonoBehaviour
{
    [SerializeField]
    private FactionSelect[] _factionSelectTabs;

    private ConPlayerFactions _conPlayerFactions;

    protected void Awake()
    {
        _conPlayerFactions = ConfactoryFinder.Instance.Get<ConPlayerFactions>();

        DEBUG_AssignFactionsToPlayersAndContinue();
    }

    private void DEBUG_AssignFactionsToPlayersAndContinue()
    {
        RegisteredPlayer[] players = ConfactoryFinder.Instance.Get<ConPlayers>().GetCurrentlyRegisteredPlayers(false);
        FactionType[] types = _conPlayerFactions.GetFreeFactions();
        for(int i = 0; i < players.Length; i++)
        {
            _conPlayerFactions.AssignPlayerToFaction(players[i], types[i]);
        }
        Invoke("GoToGameScene", 1f);
        Debug.LogError("DEBUG METHOD CALLED!: DEBUG_AssignFactionsToPlayersAndContinue", this.gameObject);
    }

    private void GoToGameScene()
    {
        ConfactoryFinder.Instance.Get<ConSceneSwitcher>().SwitchScreen(SceneNames.GAME_SCENE);
    }
}
