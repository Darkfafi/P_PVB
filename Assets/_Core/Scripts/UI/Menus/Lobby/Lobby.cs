using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ramses.Confactory;
using System;
using Ramses.SceneTrackers;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
    [Header("Options")]
    [SerializeField]
    private int _countdownTime = 5;

    [Header("Requirements")]
    [SerializeField]
    private JoinTab[] _joinTabs;

    [SerializeField]
    private Text _globalText;

    private ConPlayers _conPlayers;
    private ReadyTranslator _readyTranslator;

    private Timer _countdownTimer;

    protected void Awake()
    {
        _countdownTimer = new Timer(1, _countdownTime);
        _conPlayers = ConfactoryFinder.Instance.Get<ConPlayers>();
        _readyTranslator = SceneTrackersFinder.Instance.GetSceneTracker<AirConsoleMessageST>().Get<ReadyTranslator>();

        _conPlayers.PlayerRegisteredEvent += OnPlayerRegisteredEvent;
        _conPlayers.PlayerUnregisteredEvent += OnPlayerUnregisteredEvent;

        _countdownTimer.TimerTikkedEvent += OnTimerTikkedEvent;

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
        UnlistenToEvents();
    }

    private void UnlistenToEvents()
    {
        _conPlayers.ConPlayerReadyToUseEvent -= OnConPlayerReadyToUseEvent;

        _conPlayers.AllowPlayerRegistration(false);
        _conPlayers.AllowDeleteRegisteredPlayerOnLeave(false);

        _countdownTimer.TimerTikkedEvent -= OnTimerTikkedEvent;

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
        ChangeReadyValue(true, deviceId);
    }

    private void OnDeviceUnreadyEvent(int deviceId)
    {
        ChangeReadyValue(false, deviceId);
    }

    private void ChangeReadyValue(bool value, int deviceId)
    {
        JoinTab jt = GetJoinTabDisplaying(deviceId);
        if (jt == null) { return; }
        jt.ToggleReady(value);
        SetGlobalText();
        if (GetAmountOfTabsReady() == _conPlayers.GetCurrentlyRegisteredPlayers(true).Length && GetAmountOfTabsReady() >= 2)
            StarCountDown();
        else
            StopCountDown();
    }

    private void StarCountDown()
    {
        if (_countdownTimer.Running) { return; }
        OnTimerTikkedEvent(0);
        _countdownTimer.Start();
    }

    private void StopCountDown()
    {
        if (!_countdownTimer.Running) { return; }
        _countdownTimer.Stop();
        _countdownTimer.Reset();
    }

    private void OnTimerTikkedEvent(int timesTikked)
    {
        int timeLeft = _countdownTime - timesTikked;

        _globalText.text = "Game starts in: " + timeLeft.ToString();

        if (timeLeft <= 0)
        {
            GameStart();
        }
    }

    private void GameStart()
    {
        UnlistenToEvents();
        ConfactoryFinder.Instance.Get<ConSceneSwitcher>().SwitchScreen(SceneNames.FACTION_SCENE);
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
        int amountNeeded = 0;
        if (registeredPlayerAmount < 2)
        {
            amountNeeded = (2 - registeredPlayerAmount);
            _globalText.text = amountNeeded.ToString() + " more player"+ ((amountNeeded > 1) ? "s are" : " is")  +" required to start..";
        }
        else if(amountReady < registeredPlayerAmount)
        {
            amountNeeded = (registeredPlayerAmount - amountReady);
            _globalText.text = amountNeeded + " player"+ ((amountNeeded > 1) ? "s" :"") + " still " + ((amountNeeded > 1) ? "have" : "has") + " to ready up..";
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
        StopCountDown();
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
        SetAllPlayersReadyValue(false); // All players are set unready when a person leaves so the game cannot accidentally start without a friend.
    }

    private void SetAllPlayersReadyValue(bool value)
    {
        for(int i = 0; i < _joinTabs.Length; i++)
        {
            if (_joinTabs[i].DisplayingPlayer != null)
                ChangeReadyValue(value, _joinTabs[i].DisplayingPlayer.DeviceID);
        }
    }
}
