using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ramses.Confactory;
using System;
using Ramses.SceneTrackers;
using UnityEngine.UI;
using Ramses.SceneTrackers;

public class Lobby : MonoBehaviour {

    [SerializeField]
    private JoinTab[] _joinTabs;

    [SerializeField]
    private Text _globalText;

    private ConPlayers _conPlayers;
    private ReadyTranslator _readyTranslator;

    protected void Awake()
    {
        _conPlayers = ConfactoryFinder.Instance.Get<ConPlayers>();
        _readyTranslator = SceneTrackersFinder.Instance.GetSceneTracker<AirConsoleMessageST>().Get<ReadyTranslator>();

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

        _readyTranslator.DeviceReadyEvent -= OnDeviceReadyEvent;
        _readyTranslator.DeviceUnreadyEvent -= OnDeviceUnreadyEvent;
    }

    private void OnConPlayerReadyToUseEvent()
    {
        _conPlayers.ConPlayerReadyToUseEvent -= OnConPlayerReadyToUseEvent;
        _conPlayers.AllowPlayerRegistration(true);
        _conPlayers.AllowDeleteRegisteredPlayerOnLeave(true);


        _readyTranslator.DeviceReadyEvent += OnDeviceReadyEvent;
        _readyTranslator.DeviceUnreadyEvent += OnDeviceUnreadyEvent;

        SetGlobalText();
    }

    private void OnDeviceReadyEvent(int deviceId)
    {
        JoinTab jt = GetJoinTabDisplaying(deviceId);
        if (jt == null) { return; }
        jt.ToggleReady(true);
    }

    private void OnDeviceUnreadyEvent(int deviceId)
    {
        JoinTab jt = GetJoinTabDisplaying(deviceId);
        if (jt == null) { return; }
        jt.ToggleReady(false);
    }

    private JoinTab GetJoinTabDisplaying(int deviceId)
    {
        for(int i = 0; i < _joinTabs.Length; i++)
        {
            if (_joinTabs[i].DisplayingPlayer != null && _joinTabs[i].DisplayingPlayer.DeviceID == deviceId)
                return _joinTabs[i];
        }
        return null;
    }

    private void SetGlobalText()
    {
        int registeredPlayerAmount = _conPlayers.GetCurrentlyRegisteredPlayers(true).Length;
        int amountReady = GetAmountOfTabsReady();
        if (registeredPlayerAmount < 2)
        {
            int amountNeeded = (2 - registeredPlayerAmount);
            _globalText.text = amountNeeded.ToString() + " more player "+ ((amountNeeded > 1) ? "s" : "")  +" needed to start..";
        }
        else if(amountReady < registeredPlayerAmount)
        {
            _globalText.text = (registeredPlayerAmount - amountReady) + " players still have to ready up..";
        }
    }

    private int GetAmountOfTabsReady()
    {
        int returnValue = 0;
        for(int i = 0; i < _joinTabs.Length; i++)
        {
            if (_joinTabs[i].DisplayingPlayer != null && _joinTabs[i].IsReady)
                returnValue++;
        }

        return returnValue;
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
        SetGlobalText();
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
        SetGlobalText();
    }
}
