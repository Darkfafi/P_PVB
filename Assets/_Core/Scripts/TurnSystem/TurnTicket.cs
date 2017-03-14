using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void TurnTicketHandler(TurnTicket ticket);

public class TurnTicket
{
    public event TurnTicketHandler TurnStartedEvent;
    public event TurnTicketHandler TurnEndRequestEvent;
    public event TurnTicketHandler TurnEndedEvent;

    public int Priority { get; private set; }

    public TurnTicket()
    {

    }

    public void StartTurn()
    {
        if (TurnStartedEvent != null)
            TurnStartedEvent(this);
    }

    public void EndTurn()
    {
        if(TurnEndRequestEvent != null)
        {
            TurnEndRequestEvent(this);
        }
    }

    public void SetPriority(int priorityLevel)
    {
        if (Priority == priorityLevel) { return; }
        Priority = priorityLevel;
    }
}
