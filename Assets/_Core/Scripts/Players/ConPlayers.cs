using Ramses.Confactory;
using NDream.AirConsole;
using System.Collections.Generic;

public delegate void RegisteredPlayerHandler(RegisteredPlayer player);

public class ConPlayers : IConfactory
{
    /// <summary>
    /// Triggered when the system has connected to the AirConsole service and is ready to be used.
    /// </summary>
    public event VoidHandler ConPlayerReadyToUseEvent;

    /// <summary>
    /// Triggered for each new RegisteredPlayer. This player will be registered even when he disconnects
    /// </summary>
    public event RegisteredPlayerHandler PlayerRegisteredEvent;

    /// <summary>
    /// Triggered for each RegisteredPlayer when the list is cleaned.
    /// </summary>
    public event RegisteredPlayerHandler PlayerUnregisteredEvent;

    public const int MAX_AMOUNT_OF_REGISTERED_PLAYERS = 4;
    /// <summary>
    /// Indicates if new players are allowed for registration. 
    /// When 'false', it will only allow handeling disconnecting and reconnecting of already registered players by any device.
    /// Else if 'true', it allows also for new players up to the max amount of 'MAX_AMOUNT_OF_REGISTERED_PLAYERS'
    /// </summary>
    public bool AllowsPlayerRegistration { get; private set; }
    
    /// <summary>
    /// Indicates if the system is allowed to clean its list when a player disconnects.
    /// ALL Disconnected registered players will be removed when this is set to true / every time a player leaves.
    /// </summary>
    public bool AllowsDeleteRegisteredPlayerOnLeave { get; private set; }

    /// <summary>
    /// 'true' when the system has connected to the AirConsole service and is ready to be used.
    /// </summary>
    public bool IsReadyToUse { get; private set; }

    private RegisteredPlayer[] _registeredPlayers = new RegisteredPlayer[MAX_AMOUNT_OF_REGISTERED_PLAYERS];

    public ConPlayers()
    {
        IsReadyToUse = false;

        AirConsole.instance.onConnect += OnConnectEvent;
        AirConsole.instance.onDisconnect += OnDisconnectEvent;

        if (!AirConsole.instance.IsAirConsoleUnityPluginReady())
            AirConsole.instance.onReady += OnReadyEvent;
        else
            Ready();
    }

    public void ConClear()
    {
        AirConsole.instance.onReady -= OnReadyEvent;
        AirConsole.instance.onConnect -= OnConnectEvent;
        AirConsole.instance.onDisconnect -= OnDisconnectEvent;
        UnityEngine.Debug.Log("CONPLAYERS DEACTIVATED");
    }

    public RegisteredPlayer[] GetCurrentlyRegisteredPlayers(bool connectedOnly)
    {
        List<RegisteredPlayer> rps = new List<RegisteredPlayer>();

        for(int i= 0; i < _registeredPlayers.Length; i++)
        {
            if(_registeredPlayers[i] != null)
            {
                if (!connectedOnly || _registeredPlayers[i].IsConnected)
                    rps.Add(_registeredPlayers[i]);
            }
        }

        return rps.ToArray();
    }

    public void AllowPlayerRegistration(bool allow)
    {
        if (AllowsPlayerRegistration != allow)
        {
            AllowsPlayerRegistration = allow;
            if(AllowsPlayerRegistration)
            {
                RegisterAlreadyConnectedControllers(); // Registers already known devices after reallowing registration of players.
            }
        }
    }

    public void AllowDeleteRegisteredPlayerOnLeave(bool allow)
    {
        if(AllowsDeleteRegisteredPlayerOnLeave != allow)
        {
            AllowsDeleteRegisteredPlayerOnLeave = allow;
            if(AllowsDeleteRegisteredPlayerOnLeave)
            {
                CleanRegisteredPlayers(true);
            }
        }
    }

    public void CleanRegisteredPlayers(bool notConnectedOnly)
    {
        for(int i = _registeredPlayers.Length -1; i >=0; i--)
        {
            if (_registeredPlayers[i] == null) { continue; }

            if ((notConnectedOnly && !_registeredPlayers[i].IsConnected) || !notConnectedOnly)
            {
                _registeredPlayers[i].DeviceDisconnectAction(_registeredPlayers[i].DeviceID);
                if (PlayerUnregisteredEvent != null)
                    PlayerUnregisteredEvent(_registeredPlayers[i]);
                _registeredPlayers[i] = null;
            }
        }
    }

    private void RegisterAlreadyConnectedControllers()
    {
        int[] allControllerIds = AirConsole.instance.GetControllerDeviceIds().ToArray();

        for (int i = 0; i < allControllerIds.Length; i++)
        {
            OnConnectEvent(allControllerIds[i]);
        }
    }

    private void OnReadyEvent(string code)
    {
        Ready();
    }

    /// <summary>
    /// Sets the max player amount and looks for the connected controllers to create players for.
    /// </summary>
    private void Ready()
    {
        AirConsole.instance.onReady -= OnReadyEvent;
        IsReadyToUse = true;
        AirConsole.instance.SetActivePlayers(MAX_AMOUNT_OF_REGISTERED_PLAYERS);

        UnityEngine.Debug.Log("CONPLAYERS ACTIVATED");

        if (ConPlayerReadyToUseEvent != null)
            ConPlayerReadyToUseEvent();

        RegisterAlreadyConnectedControllers();
    }

    /// <summary>
    /// Gets a registered player by its device id. If there is no player with this device id it returns null.
    /// </summary>
    /// <param name="device_id">The deviceId the caller wants a registered player for as return value. </param>
    /// <returns>Found player with device id. Returns null if no player with device_id is found.</returns>
    public RegisteredPlayer GetRegisteredPlayerById(int device_id)
    {
        for (int i = 0; i < _registeredPlayers.Length; i++)
        {
            if (_registeredPlayers[i] != null && _registeredPlayers[i].DeviceID == device_id)
            {
                return _registeredPlayers[i];
            }
        }
        return null;
    }

    private void OnConnectEvent(int device_id)
    {
        RegisteredPlayer rd = GetRegisteredPlayerById(device_id);

        if (rd != null)
        {
            rd.DeviceConnectedAction(device_id);
            // Same device came back for player
            return;
        }
        else if(AllowsPlayerRegistration)
        {
            for (int i = 0; i < _registeredPlayers.Length; i++)
            {
                if (_registeredPlayers[i] == null)
                {
                    _registeredPlayers[i] = new RegisteredPlayer(i, device_id);
                    // new register player item made
                    if (PlayerRegisteredEvent != null)
                        PlayerRegisteredEvent(_registeredPlayers[i]);
                    return;
                }
            }
        }

        // If the device was not registered as a new or reconnected as a lost device -
        // -> then find disconnected player and register the new user as him
        for (int i = 0; i < _registeredPlayers.Length; i++)
        {
            if (_registeredPlayers[i] != null && !_registeredPlayers[i].IsConnected)
            {
                // Other device came to replace player
                _registeredPlayers[i].LinkDeviceToPlayer(device_id);
                return;
            }
        }
    }

    private void OnDisconnectEvent(int device_id)
    {
        RegisteredPlayer rd = GetRegisteredPlayerById(device_id);

        if (rd != null)
            rd.DeviceDisconnectAction(device_id);

        if (AllowsDeleteRegisteredPlayerOnLeave)
            CleanRegisteredPlayers(true);
    }
}
