using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using NDream.AirConsole;

public class PhasesTranslator : BaseACMessageTranslator
{
    public void UpdateOnCurrentPhase(GamePhase gamePhase, int deviceId)
    {
        var message = new
        {
            action = "gamePhaseUpdate",
            info = new { gamePhase = gamePhase.ToString() }
        };

        AirConsole.instance.Message(deviceId, message);   
    }

    protected override void MessageReceived(int from, JToken data)
    {
        
    }
}
