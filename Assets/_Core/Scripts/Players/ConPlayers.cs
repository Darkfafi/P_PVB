using Ramses.Confactory;
using NDream.AirConsole;
using System;

public class ConPlayers : IConfactory
{
    public const int MAX_AMOUNT_OF_REGISTERED_PLAYERS = 4;

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

    private void Ready()
    {
        AirConsole.instance.onReady -= OnReadyEvent;

        AirConsole.instance.SetActivePlayers(MAX_AMOUNT_OF_REGISTERED_PLAYERS);
        RegisterAlreadyConnectedControllers();

        UnityEngine.Debug.Log("CONPLAYERS ACTIVATED");
    }

    private void OnConnectEvent(int device_id)
    {
        RegisteredPlayer rd = GetRegisteredPlayerById(device_id);

        if (rd != null)
        {
            rd.DeviceConnectedAction(device_id);
            return;
        }
        else
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

    private void OnDisconnectEvent(int device_id)
    {
        RegisteredPlayer rd = GetRegisteredPlayerById(device_id);

        if (rd != null)
            rd.DeviceDisconnectAction(device_id);
    }
}
