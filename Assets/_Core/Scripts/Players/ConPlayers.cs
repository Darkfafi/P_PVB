using Ramses.Confactory;
using NDream.AirConsole;

public class ConPlayers : IConfactory
{
    public const int MAX_AMOUNT_OF_REGISTERED_PLAYERS = 4;

    /// <summary>
    /// Indicates if new players are allowed for registration. 
    /// When 'false', it will only allow handeling disconnecting and reconnecting of already registered players by any device.
    /// Else if 'true', it allows also for new players up to the max amount of 'MAX_AMOUNT_OF_REGISTERED_PLAYERS'
    /// </summary>
    public bool AllowsPlayerRegistration { get; private set; }

    private RegisteredPlayer[] _registeredPlayers = new RegisteredPlayer[MAX_AMOUNT_OF_REGISTERED_PLAYERS];

    public ConPlayers()
    {
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

    public RegisteredPlayer[] GetRegisteredPlayers()
    {
        RegisteredPlayer[] rps = _registeredPlayers;
        return rps;
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

        AirConsole.instance.SetActivePlayers(MAX_AMOUNT_OF_REGISTERED_PLAYERS);
        RegisterAlreadyConnectedControllers();

        UnityEngine.Debug.Log("CONPLAYERS ACTIVATED");
    }

    /// <summary>
    /// Gets a registered player by its device id. If there is no player with this device id it returns null.
    /// </summary>
    /// <param name="device_id">The deviceId the caller wants a registered player for as return value. </param>
    /// <returns>Found player with device id. Returns null if no player with device_id is found.</returns>
    private RegisteredPlayer GetRegisteredPlayerById(int device_id)
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
            return;
        }
        else if(AllowsPlayerRegistration)
        {
            for (int i = 0; i < _registeredPlayers.Length; i++)
            {
                if (_registeredPlayers[i] == null)
                {
                    _registeredPlayers[i] = new RegisteredPlayer(i, device_id);
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
    }
}
