using Newtonsoft.Json.Linq;
using UnityEngine;

/// <summary>
/// Handles communication between the controller and the game about coin actions.
/// </summary>
public class CoinTranslator : BaseACMessageTranslator
{
    public event DoubleIntHandler CoinRequestEvent;

    protected override void MessageReceived(int from, JToken data)
    {
        if (SendEventIfCoinRequest(from, data)) { return; }
    }

    private bool SendEventIfCoinRequest(int from, JToken data)
    {
        if(data["coinRequest"] != null)
        {
            if(data["coinRequest"]["coinAmount"] != null)
            {
                if (CoinRequestEvent != null)
                    CoinRequestEvent((int)data["coinRequest"]["coinAmount"], from);
            }
            else
            {
                Debug.LogError("No info found for request: 'coinRequest'");
            }
            return true;
        }
        return false;
    }
}
