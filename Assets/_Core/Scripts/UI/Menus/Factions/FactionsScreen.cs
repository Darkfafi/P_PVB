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
}
