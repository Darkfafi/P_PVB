using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ramses.Confactory;
using System;
using Ramses.SceneTrackers;
using UnityEngine.UI;

public class Lobby : MonoBehaviour {

    [SerializeField]
    private JoinTab[] _joinTabs;

    [SerializeField]
    private Text _globalText;

    private ConPlayers _conPlayers;

    protected void Awake()
    {
        _conPlayers = ConfactoryFinder.Instance.Get<ConPlayers>();

        _conPlayers.PlayerRegisteredEvent += OnPlayerRegisteredEvent;
        _conPlayers.PlayerUnregisteredEvent += OnPlayerUnregisteredEvent;

        if (!_conPlayers.IsReadyToUse)
        {
            _conPlayers.ConPlayerReadyToUseEvent += OnConPlayerReadyToUseEvent;
        }
        else
        {
            OnConPlayerReadyToUseEvent();
        }
    }

    protected void OnDestroy()
    {
        _conPlayers.ConPlayerReadyToUseEvent -= OnConPlayerReadyToUseEvent;

        _conPlayers.AllowPlayerRegistration(false);
        _conPlayers.AllowDeleteRegisteredPlayerOnLeave(false);

        _conPlayers.PlayerRegisteredEvent -= OnPlayerRegisteredEvent;
        _conPlayers.PlayerUnregisteredEvent -= OnPlayerUnregisteredEvent;
    }

    private void OnConPlayerReadyToUseEvent()
    {
        _conPlayers.ConPlayerReadyToUseEvent -= OnConPlayerReadyToUseEvent;
        _conPlayers.AllowPlayerRegistration(true);
        _conPlayers.AllowDeleteRegisteredPlayerOnLeave(true);

        SetGlobalText();
    }

    private void SetGlobalText()
    {
        int registeredPlayerAmount = _conPlayers.GetCurrentlyRegisteredPlayers(false).Length;
        if (registeredPlayerAmount < 2)
        {
            _globalText.text = (2 - registeredPlayerAmount).ToString() + " more players needed to start..";
        }
    }

    private void OnPlayerRegisteredEvent(RegisteredPlayer player)
    {
        for(int i = 0; i < _joinTabs.Length; i++)
        {
            if(_joinTabs[i].DisplayingPlayer == null)
            {
                _joinTabs[i].DisplayPlayer(player);
                break;
            }
        }
    }

    private void OnPlayerUnregisteredEvent(RegisteredPlayer player)
    {
        bool replacingPlayers = false;
        for(int i = 0; i < _joinTabs.Length; i++)
        {
            if (!replacingPlayers)
            {
                if (_joinTabs[i].DisplayingPlayer == player)
                {
                    _joinTabs[i].DisplayPlayer(null);
                    replacingPlayers = true;
                    i = -1;
                }
            }
            else
            {
                if(_joinTabs[i].DisplayingPlayer == null)
                {
                    for(int j = i; j < _joinTabs.Length; j++)
                    {
                        if(_joinTabs[j].DisplayingPlayer != null)
                        {
                            _joinTabs[i].DisplayPlayer(_joinTabs[j].DisplayingPlayer);
                            _joinTabs[j].DisplayPlayer(null);
                            break;
                        }
                    }
                }
            }
        }
    }
}
