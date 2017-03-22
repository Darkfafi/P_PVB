using UnityEngine;
using Ramses.Confactory;
using Ramses.SceneTrackers;
using UnityEngine.UI;
using System;

/// <summary>
/// This component binds all the functionalities to form the Lobby screen.
/// </summary>
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
            _conPlayers.CleanRegisteredPlayers(true);
            OnConPlayerReadyToUseEvent();
        }
    }

    protected void OnDestroy()
    {
        UnlistenToEvents();
    }

    /// <summary>
    /// Unlistenes from all events listening to.
    /// </summary>
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

    /// <summary>
    /// When the ConPlayers ready event is triggered, this method will be called.
    /// </summary>
    private void OnConPlayerReadyToUseEvent()
    {
        _conPlayers.ConPlayerReadyToUseEvent -= OnConPlayerReadyToUseEvent;
        _conPlayers.AllowPlayerRegistration(true);
        _conPlayers.AllowDeleteRegisteredPlayerOnLeave(true);


        _readyTranslator.DeviceReadyEvent += OnDeviceReadyEvent;
        _readyTranslator.DeviceUnreadyEvent += OnDeviceUnreadyEvent;

        SetGlobalText();
    }

    /// <summary>
    /// When a device is ready, this will toggle the player to ready.
    /// </summary>
    /// <param name="deviceId">Device id of player which pressed ready</param>
    private void OnDeviceReadyEvent(int deviceId)
    {
        ChangeReadyValue(true, deviceId);
    }


    /// <summary>
    /// When a device is unready, this will toggle the player to unready.
    /// </summary>
    /// <param name="deviceId">Device id of player which pressed ready</param>
    private void OnDeviceUnreadyEvent(int deviceId)
    {
        ChangeReadyValue(false, deviceId);
    }

    /// <summary>
    /// This will set the ready state of the given player.
    /// </summary>
    /// <param name="value">Ready state</param>
    /// <param name="deviceId">DeviceID of player</param>
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

    /// <summary>
    /// Starts the countdown to go to the next scene.
    /// </summary>
    private void StarCountDown()
    {
        if (_countdownTimer.Running) { return; }
        OnTimerTikkedEvent(0);
        _countdownTimer.Start();
    }

    /// <summary>
    /// Stops the countdown to go to the next scene.
    /// </summary>
    private void StopCountDown()
    {
        if (!_countdownTimer.Running) { return; }
        _countdownTimer.Stop();
        _countdownTimer.Reset();
    }

    /// <summary>
    /// When the timer makes a tick it will trigger this method. When the timer is done, this method will direct the game to the next scene.
    /// This method will also set the global text to the time left until the scene switches.
    /// </summary>
    /// <param name="timesTikked"></param>
    private void OnTimerTikkedEvent(int timesTikked)
    {
        int timeLeft = _countdownTime - timesTikked;

        _globalText.text = "Game starts in: " + timeLeft.ToString();

        if (timeLeft <= 0)
        {
            GameStart();
        }
    }
    /// <summary>
    /// This directs the game to the next scene, the Faction Scene.
    /// </summary>
    private void GameStart()
    {
        UnlistenToEvents();
        ConfactoryFinder.Instance.Get<ConSceneSwitcher>().SwitchScreen(SceneNames.FACTION_SCENE);
    }

    /// <summary>
    /// Returns the JoinTab which displays the player with the device id given as parameter.
    /// </summary>
    /// <param name="deviceId">DeviceId of the player represented in the tab</param>
    /// <returns>The joinTab of player. If there was no JoinTab found for the given player then it returns null</returns>
    private JoinTab GetJoinTabDisplaying(int deviceId)
    {
        for(int i = 0; i < _joinTabs.Length; i++)
        {
            if (_joinTabs[i].DisplayingPlayer != null && _joinTabs[i].DisplayingPlayer.DeviceID == deviceId)
                return _joinTabs[i];
        }
        return null;
    }

    /// <summary>
    /// Sets the global text to display instructions to the players
    /// </summary>
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

    /// <summary>
    /// Returns the amount of tabs which have a 'ready' state.
    /// </summary>
    /// <returns>The amount of tabs which are ready</returns>
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

    /// <summary>
    /// When a player is registered, it will link the player to a JoinTab.
    /// </summary>
    /// <param name="player">The registered player</param>
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

    /// <summary>
    /// When a player is unregisterd, it will unlink the player from his or her JoinTab.
    /// </summary>
    /// <param name="player">The unregistered player</param>
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

    /// <summary>
    /// Sets all the JoinTabs to the ready state.
    /// </summary>
    /// <param name="value"></param>
    private void SetAllPlayersReadyValue(bool value)
    {
        for(int i = 0; i < _joinTabs.Length; i++)
        {
            if (_joinTabs[i].DisplayingPlayer != null)
                ChangeReadyValue(value, _joinTabs[i].DisplayingPlayer.DeviceID);
        }
    }
}
