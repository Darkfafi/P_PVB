using Newtonsoft.Json.Linq;
using UnityEngine;
using NDream.AirConsole;

public delegate void DeviceFactionTypeHandler(int deviceId, FactionType factionType, bool selectType);

/// <summary>
/// This can be used tor the communication between the controller and the Game in context of the Factions (Setting, Updating)
/// </summary>
public class FactionsTranslator : BaseACMessageTranslator
{
    /// <summary>
    /// The event is triggered when a device-message has been received on assigning / unassigning factions to / from players
    /// </summary>
    public event DeviceFactionTypeHandler FactionRequestEvent;

    /// <summary>
    /// Sends to the controllers that all the available Factions must be updated
    /// </summary>
    /// <param name="indexesOfFactionsAvailable"></param>
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

    /// <summary>
    /// This method checks if the message received is a faction assignment / unassignment message
    /// If true, this will send an event in responce.
    /// </summary>
    /// <param name="from">The device the message is from</param>
    /// <param name="data">The data the device has sent.</param>
    /// <returns>returns true if the message is an assignment / unassignment message</returns>
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
