using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem
{
    public event IntHandler TurnStartedEvent;
    public event IntHandler TurnEndedEvent;
    public event VoidHandler TurnSystemStartedEvent;
    public event VoidHandler TurnSystemEndedEvent;

    public bool IsLooping { get; private set; }
    public int CurrentTurnTicket { get { return (_currentTurnUserIndex >= 0 && _turnTickets.Count > 0) ? _turnTickets[_currentTurnUserIndex].TicketId : -1; } }
    private TurnTicket _currentTurnTicket { get { return (_currentTurnUserIndex >= 0 && _turnTickets.Count > 0) ? _turnTickets[_currentTurnUserIndex] : null; } }

    private List<TurnTicket> _turnTickets = new List<TurnTicket>();
    private int _currentTurnUserIndex = -1;

    public TurnSystem(bool looping)
    {
        IsLooping = looping;
    }

    public int CurrentTicketDistanceFromLastTicket()
    {
        if (_currentTurnTicket == null) { return -1; }
        return (_turnTickets.Count - 1) - _currentTurnUserIndex;
    }
    
    public void AddTurnTickets(params int[] turnTickets)
    {
        for(int i = 0; i < turnTickets.Length; i++)
        {
            if (GetTicketWithId(turnTickets[i]) == null)
            {
                _turnTickets.Add(new TurnTicket(turnTickets[i]));
            }
        }
    }

    public void SetPriorityLevelOfTicket(int ticket, int priorityLevel)
    {
        TurnTicket tt = GetTicketWithId(ticket);
        if (tt != null)
            tt.SetPriorityLevel(priorityLevel);
        else
            Debug.LogWarning("No ticket found with id: " + ticket.ToString());
    }

    public int GetPriorityLevelOfTicket(int ticket)
    {
        TurnTicket tt = GetTicketWithId(ticket);
        return (tt != null) ? tt.PriorityLevel : -1;
    }

    public void RemoveTurnTickets(params int[] turnTickets)
    {
        TurnTicket tt;
        for (int i = 0; i < turnTickets.Length; i++)
        {
            tt = GetTicketWithId(turnTickets[i]);
            if (tt != null)
                _turnTickets.Remove(tt);
        }

        _currentTurnUserIndex = _turnTickets.GetClampedIndex(_currentTurnUserIndex);
    }

    public void ClearTurnTickets()
    {
        _turnTickets.Clear();
        _currentTurnUserIndex = -1;
    }

    public void StartTurnSystem()
    {
        _currentTurnUserIndex = -1;
        if (TurnSystemStartedEvent != null)
            TurnSystemStartedEvent();

        GiveNextTicketTurn();
    }

    public void StopTurnSystem()
    {
        if (_currentTurnTicket != null)
            TakeTurnAwayFromTicket(_currentTurnTicket);

        if (TurnSystemEndedEvent != null)
            TurnSystemEndedEvent();
    }

    public void EndTurnForCurrentTicket()
    {
        GiveNextTicketTurn();
    }

    private void GiveNextTicketTurn()
    {
        _turnTickets.Sort(PrioritySort);

        if(_currentTurnTicket != null)
            TakeTurnAwayFromTicket(_currentTurnTicket);

        if (_currentTurnUserIndex == (_turnTickets.Count - 1) && !IsLooping)
        {
            StopTurnSystem();
            return;
        }

        _currentTurnUserIndex = _turnTickets.GetLoopIndex(_currentTurnUserIndex + 1);
        GiveTicketTurn(_currentTurnTicket);
    }

    private void TakeTurnAwayFromTicket(TurnTicket ticket)
    {
        if (TurnEndedEvent != null)
            TurnEndedEvent(ticket.TicketId);
    }

    private void GiveTicketTurn(TurnTicket ticket)
    {
        if (TurnStartedEvent != null)
            TurnStartedEvent(ticket.TicketId);
    }

    private int PrioritySort(TurnTicket x, TurnTicket y)
    {
        if (x.PriorityLevel > y.PriorityLevel)
            return 1;
        return -1;
    }

    private TurnTicket GetTicketWithId(int id)
    {
        for(int i = 0; i < _turnTickets.Count; i++)
        {
            if (_turnTickets[i].TicketId == id)
                return _turnTickets[i];
        }
        return null;
    }

    private class TurnTicket
    {
        public int TicketId { get; private set; }
        public int PriorityLevel { get; private set; }

        public TurnTicket(int id)
        {
            TicketId = id;
            PriorityLevel = 0;
        }

        public TurnTicket(int id, int priorityLevel)
        {
            TicketId = id;
            PriorityLevel = priorityLevel;
        }

        public void SetPriorityLevel(int level)
        {
            Debug.Log(level + " < --- level");
            PriorityLevel = level;
        }
    }
}
