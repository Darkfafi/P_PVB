using UnityEngine;
using NDream.AirConsole;

public class RegisteredPlayer
{
    public int DeviceID { get; private set; }
    public int PlayerIndex { get; private set; }
    public bool IsConnected { get; private set; }

    public RegisteredPlayer(int playerIndex, int deviceId)
    {
        PlayerIndex = playerIndex;
        LinkDeviceToPlayer(deviceId);
    }

    public void LinkDeviceToPlayer(int deviceId)
    {
        if (deviceId == 0) { return; } // Player can't be main screen. (Main screen always == deviceId 0) // https://developers.airconsole.com/#!/guides/api_methods_overview
        DeviceID = deviceId;
        if (IsActiveDevice())
            DeviceConnectedAction(DeviceID);
    }

    public void DeviceConnectedAction(int deviceId)
    {
        if (deviceId != DeviceID || IsConnected) { return; }
        IsConnected = true;
        Debug.LogWarning("Device Connected: " + deviceId + " PlayerIndex" + PlayerIndex);
    }

    public void DeviceDisconnectAction(int deviceId)
    {
        if (deviceId != DeviceID || !IsConnected) { return; }
        IsConnected = false;
    }

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
