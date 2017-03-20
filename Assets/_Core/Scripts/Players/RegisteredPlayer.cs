using UnityEngine;
using NDream.AirConsole;

/// <summary>
/// The registered player can be seen as the chair of the player around the table.
/// This class has information on who is sitting on the chair, if that person is currently sitting or not, events for when he sits or leaves and much more.
/// </summary>
public class RegisteredPlayer
{
    /// <summary>
    /// This event is fired when a device is connected to the registerdPlayer
    /// </summary>
    public event RegisteredPlayerHandler RegisteredPlayerConnectedEvent;

    /// <summary>
    /// This event is fired when a device is disconnected from the registerdPlayer
    /// </summary>
    public event RegisteredPlayerHandler RegisteredPlayerDisconnectedEvent;

    /// <summary>
    /// The connected device to the RegisteredPlayer. 
    /// If the player is disconnected, the deviceId stays the same but the 'IsConnected' turns to 'false'
    /// </summary>
    public int DeviceID { get; private set; }
    /// <summary>
    /// The player index indicates if the player is 'Player one', or 'Player two' etc. 
    /// This will not change as long as this registered player is not deleted from the 'ConPlayers' system
    /// </summary>
    public int PlayerIndex { get; private set; }

    /// <summary>
    /// Indicates to if the player is connected or not
    /// </summary>
    public bool IsConnected { get; private set; }

    /// <summary>
    /// The creation of the RegisteredPlayers needs a first device to be linked to and a player index to know its place in the game.
    /// </summary>
    /// <param name="playerIndex">Player index, (Being player one, player two etc..)</param>
    /// <param name="deviceId">Device first linked to the player</param>
    public RegisteredPlayer(int playerIndex, int deviceId)
    {
        PlayerIndex = playerIndex;
        LinkDeviceToPlayer(deviceId);
    }

    /// <summary>
    /// Links a new device to the RegisteredPlayer. deviceId == 0 can NOT be assigned due to it being the screen itself.
    /// </summary>
    /// <param name="deviceId">Device id of the new device to link to the player</param>
    public void LinkDeviceToPlayer(int deviceId)
    {
        if (deviceId == 0) { return; } // Player can't be main screen. (Main screen always == deviceId 0) // https://developers.airconsole.com/#!/guides/api_methods_overview
        DeviceID = deviceId;
        if (IsActiveDevice())
            DeviceConnectedAction(DeviceID);
    }

    /// <summary>
    /// This method is called by the 'ConPlayers' to communicate a new device is connected to the RegisteredPlayer
    /// </summary>
    /// <param name="deviceId">Device id of the new device to link to the player</param>
    public void DeviceConnectedAction(int deviceId)
    {
        if (deviceId != DeviceID || IsConnected) { return; }
        IsConnected = true;

        if (RegisteredPlayerConnectedEvent != null)
            RegisteredPlayerConnectedEvent(this);

        Debug.LogWarning("Device Connected: " + deviceId + " PlayerIndex" + PlayerIndex);
    }

    /// <summary>
    /// This method is called by the 'ConPlayers' to communicate a the linked device has been disconnected from the RegisteredPlayer
    /// </summary>
    /// <param name="deviceId"></param>
    public void DeviceDisconnectAction(int deviceId)
    {
        if (deviceId != DeviceID || !IsConnected) { return; }
        IsConnected = false;
        if (RegisteredPlayerDisconnectedEvent != null)
            RegisteredPlayerDisconnectedEvent(this);
    }

    /// <summary>
    /// Checks if the current device is an active device (To check its own 'IsConnected' boolean is correct)
    /// </summary>
    /// <returns></returns>
    private bool IsActiveDevice()
    {
        int[] allControllerIds = AirConsole.instance.GetControllerDeviceIds().ToArray();

        for (int i = 0; i < allControllerIds.Length; i++)
        {
            if (allControllerIds[i] == DeviceID)
                return true;
        }

        return false;
    }
}
