using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class ReadyTranslator : BaseACMessageTranslator
{
    public event IntHandler DeviceReadyEvent;
    public event IntHandler DeviceUnreadyEvent;

    protected override void MessageReceived(int from, JToken data)
    {
        if (SendEventIfReadyMessage(from, data)) { return; }
    }

    private bool SendEventIfReadyMessage(int from, JToken data)
    { 
        if(data["readyAction"] != null)
        {
            if(data["readyAction"]["readyState"] != null)
            {
                if((bool)data["readyAction"]["readyState"])
                {
                    if (DeviceReadyEvent != null)
                        DeviceReadyEvent(from);
                }
                else
                {
                    if (DeviceUnreadyEvent != null)
                        DeviceUnreadyEvent(from);
                }
            }
            else
            {
                Debug.LogError("NO INFO PARAMETER IS GIVEN FOR THIS TRANSLATOR");
            }
            return true;
        }
        return false;
    }
}
