using Newtonsoft.Json.Linq;
using UnityEngine;
using NDream.AirConsole;

public delegate void DeviceFactionTypeHandler(int deviceId, FactionType factionType, bool selectType);

public class FactionsTranslator : BaseACMessageTranslator
{
    public event DeviceFactionTypeHandler FactionRequestEvent;

    public void SendUpdateFactionsAvailable(params int[] indexesOfFactionsAvailable)
    {
        string listOfAvailableFactionsIndexes = CreateParsableString(indexesOfFactionsAvailable);
        var message = new
        {
            action = "UpdateFactionsAvailable",
            info = new { factionIndexes = listOfAvailableFactionsIndexes }
        };
        RegisteredPlayer[] players = Ramses.Confactory.ConfactoryFinder.Instance.Get<ConPlayers>().GetCurrentlyRegisteredPlayers(true);
        for(int i = 0; i < players.Length; i++)
        {
            AirConsole.instance.Message(players[i].DeviceID, message);
        }
    }

    protected override void MessageReceived(int from, JToken data)
    {
        if (SendEventIfFactionMessage(from, data)) { return; }
    }

    private bool SendEventIfFactionMessage(int from, JToken data)
    {
        if(data["factionAction"] != null)
        {
            if (data["factionAction"]["factionIndex"] != null && data["factionAction"]["selectType"] != null)
            {
                FactionType factionRequested = (FactionType)((int.Parse((string)data["factionAction"]["factionIndex"]) + 1));
                bool selectType = (bool)data["factionAction"]["selectType"];
                if (FactionRequestEvent != null)
                {
                    FactionRequestEvent(from, factionRequested, selectType);
                }
            }
            else
            {
                Debug.LogError("No info for faction translation");
            }
            return true;
        }
        return false;
    }
}
