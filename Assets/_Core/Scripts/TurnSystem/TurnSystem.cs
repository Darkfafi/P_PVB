using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem
{
    public event TurnTicketHandler TurnStartedEvent;
    public event TurnTicketHandler TurnEndedEvent;
    public event VoidHandler TurnSystemStartedEvent;
    public event VoidHandler TurnSystemEndedEvent;

    public bool IsLooping { get; private set; }
    public TurnTicket CurrentTurnTicket { get { return (currentTurnUserIndex >= 0 && _turnTickets.Count > 0) ? _turnTickets[currentTurnUserIndex] : null; } }

    private List<TurnTicket> _turnTickets = new List<TurnTicket>();

    private int currentTurnUserIndex = -1;

    public void AddTurnTickets(params TurnTicket[] turnTickets)
    {
        for(int i = 0; i < turnTickets.Length; i++)
        {
            if (!_turnTickets.Contains(turnTickets[i]))
            {
                _turnTickets.Add(turnTickets[i]);
            }
        }
    }

    private void OnRequestMethod(TurnTicket obj)
    {
        if(obj == CurrentTurnTicket)
        {
            GiveNextTicketTurn();
        }
    }

    public void RemoveTurnTickets(params TurnTicket[] turnTickets)
    {
        for (int i = 0; i < turnTickets.Length; i++)
        {
            if (_turnTickets.Contains(turnTickets[i]))
                _turnTickets.Remove(turnTickets[i]);
        }

        currentTurnUserIndex = _turnTickets.GetClampedIndex(currentTurnUserIndex);
    }

    public void ClearTurnTickets()
    {
        _turnTickets.Clear();
        currentTurnUserIndex = -1;
    }

    public void StartTurnSystem()
    {
        GiveNextTicketTurn();
    }

    public void StopTurnSystem()
    {
        if (CurrentTurnTicket != null)
            CurrentTurnTicket.EndTurn();

        currentTurnUserIndex = -1;
    }

    private void GiveNextTicketTurn()
    {
        _turnTickets.Sort(PrioritySort);

        if (CurrentTurnTicket != null)
            CurrentTurnTicket.EndTurn();

        currentTurnUserIndex = _turnTickets.GetLoopIndex(currentTurnUserIndex + 1);
        CurrentTurnTicket.StartTurn();
    }

    private int PrioritySort(TurnTicket x, TurnTicket y)
    {
        if (x.Priority > y.Priority)
            return 1;
        return -1;
    }
}
