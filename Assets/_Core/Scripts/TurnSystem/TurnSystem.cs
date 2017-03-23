using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Turnsystem is a system which gives turns to Ids in a given order. It will also sort them on prioritylevel.
/// You can only add 1 of every id to the turnSystem as turnTicket. The 'StartTurnSystem' must be called to run it.
/// If the turnsystem is looping, the turnsystem will keep on giving turns until it is ended by an outside source, else it will end itself after the last ticket's turn has been ended.
/// The design for the system is that an outside system uses this to do game specific turn assigning / manipulation
/// </summary>
public class TurnSystem
{
    /// <summary>
    /// This event is triggered when a TurnTicket is given its turn.
    /// </summary>
    public event IntHandler TurnStartedEvent;
    /// <summary>
    /// This event is triggered when the TurnTicket has lost its turn.
    /// </summary>
    public event IntHandler TurnEndedEvent;
    /// <summary>
    /// This event is triggered when the turnSystem is started
    /// </summary>
    public event VoidHandler TurnSystemStartedEvent;
    /// <summary>
    /// This event is triggered when the turnSystem is ended.
    /// Note: This can happen manually but also when there is no looping active and the last TurnTicket's turn has been ended.
    /// </summary>
    public event VoidHandler TurnSystemEndedEvent;

    /// <summary>
    /// Indicates if the turnSystem is looping. 
    /// If false, the turnSystem will stop after reaching the last turnTicket
    /// </summary>
    public bool IsLooping { get; private set; }
    /// <summary>
    /// The Current Turn Ticket its turn by its given ID
    /// If there is no ticket which has a turn, this will return the value '-1'
    /// </summary>
    public int CurrentTurnTicket { get { return (_currentTurnUserIndex >= 0 && _turnTickets.Count > 0) ? _turnTickets[_currentTurnUserIndex].TicketId : -1; } }
    private TurnTicket _currentTurnTicket { get { return (_currentTurnUserIndex >= 0 && _turnTickets.Count > 0) ? _turnTickets[_currentTurnUserIndex] : null; } }

    private List<TurnTicket> _turnTickets = new List<TurnTicket>();
    private int _currentTurnUserIndex = -1;

    /// <summary>
    /// If the turnsystem is looping, the turnsystem will keep on giving turns until it is ended by an outside source, else it will end itself after the last ticket's turn has been ended.
    /// </summary>
    /// <param name="looping">Sets the TurnSystem to looping or not looping.</param>
    public TurnSystem(bool looping)
    {
        IsLooping = looping;
    }

    /// <summary>
    /// This returns how many turns this turn ticket is away from the last ticket.
    /// </summary>
    /// <returns>Amount of turns away</returns>
    public int CurrentTicketDistanceFromLastTicket()
    {
        if (_currentTurnTicket == null) { return -1; }
        return (_turnTickets.Count - 1) - _currentTurnUserIndex;
    }
    
    /// <summary>
    /// Adds TurnTickets to the system by their ids
    /// </summary>
    /// <param name="turnTickets">Id's to add to track as turnTickets</param>
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
    /// <summary>
    /// Set the priorityLevel of tickets. The higher the priorityLevel, the sooner the ticket gets its turn in the sorting algoritm.
    /// </summary>
    /// <param name="ticket">Ticket to change priorityLevel of</param>
    /// <param name="priorityLevel">The level to assign it</param>
    public void SetPriorityLevelOfTicket(int ticket, int priorityLevel)
    {
        TurnTicket tt = GetTicketWithId(ticket);
        if (tt != null)
            tt.SetPriorityLevel(priorityLevel);
        else
            Debug.LogWarning("No ticket found with id: " + ticket.ToString());
    }

    /// <summary>
    /// This method returns the priorityLevel of the requested ticket.
    /// </summary>
    /// <param name="ticket">Ticket to know the PriorityLevel of</param>
    /// <returns></returns>
    public int GetPriorityLevelOfTicket(int ticket)
    {
        TurnTicket tt = GetTicketWithId(ticket);
        return (tt != null) ? tt.PriorityLevel : -1;
    }

    /// <summary>
    /// This method removes TurnTickets from the system.
    /// </summary>
    /// <param name="turnTickets">TurnTicket to remove by its id</param>
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

    /// <summary>
    /// This clears the entire list of all the TurnTickets it was given.
    /// </summary>
    public void ClearTurnTickets()
    {
        _turnTickets.Clear();
        _currentTurnUserIndex = -1;
    }

    /// <summary>
    /// This starts the turn system.
    /// If not looping, this will end when it has ended the last ticket its turn.
    /// </summary>
    public void StartTurnSystem()
    {
        _currentTurnUserIndex = -1;
        if (TurnSystemStartedEvent != null)
            TurnSystemStartedEvent();

        GiveNextTicketTurn();
    }

    /// <summary>
    /// This stops the turn system.
    /// </summary>
    public void StopTurnSystem()
    {
        if (_currentTurnTicket != null)
            TakeTurnAwayFromTicket(_currentTurnTicket);

        if (TurnSystemEndedEvent != null)
            TurnSystemEndedEvent();
    }

    /// <summary>
    /// This ends the turn for the current TurnTicket
    /// </summary>
    public void EndTurnForCurrentTicket()
    {
        GiveNextTicketTurn();
    }

    /// <summary>
    /// This sorts the list on priorityLevel, ends the current ticket its turn and gives turn to the next turnTicket
    /// </summary>
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

    /// <summary>
    /// This ends the turn for the given turnTicket
    /// </summary>
    /// <param name="ticket">Ticket to take turn from</param>
    private void TakeTurnAwayFromTicket(TurnTicket ticket)
    {
        if (TurnEndedEvent != null)
            TurnEndedEvent(ticket.TicketId);
    }
    /// <summary>
    /// This gives turn to given ticket.
    /// </summary>
    /// <param name="ticket">Ticket to give turn to</param>
    private void GiveTicketTurn(TurnTicket ticket)
    {
        if (TurnStartedEvent != null)
            TurnStartedEvent(ticket.TicketId);
    }

    /// <summary>
    /// The sorting algoritm for the priorityLevel in each turnTicket.
    /// The higher the priorityLevel, the earlier in the list it gets placed.
    /// </summary>
    /// <param name="x">fist tested turnTicket</param>
    /// <param name="y">Second tested turnTicket</param>
    /// <returns></returns>
    private int PrioritySort(TurnTicket x, TurnTicket y)
    {
        if (x.PriorityLevel > y.PriorityLevel)
            return 1;
        return -1;
    }

    /// <summary>
    /// This returns a turnTicket by its id.
    /// </summary>
    /// <param name="id">The id of the turnTicket to find</param>
    /// <returns>TurnTicket found by id</returns>
    private TurnTicket GetTicketWithId(int id)
    {
        for(int i = 0; i < _turnTickets.Count; i++)
        {
            if (_turnTickets[i].TicketId == id)
                return _turnTickets[i];
        }
        return null;
    }

    /// <summary>
    /// The turnTicket class which is a dataContainer for all the info about the turnTickets.
    /// The class is private because only the system should know about them.
    /// </summary>
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
