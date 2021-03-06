﻿using System.Collections.Generic;
using UnityEngine;
using Ramses.Confactory;
using System;

public delegate void PlayerFactionHandler(PlayerFactionLinkItem linkItem);

public class ConPlayerFactions : IConfactory
{
    public event PlayerFactionHandler PlayerFactionAssignedEvent;
    public event PlayerFactionHandler PlayerFactionUnassignedEvent;

    public FactionsLibrary FactionsLibrary { get; private set; }
    public PlayerFactionLinkItem[] PlayerFactionLinks { get; private set; }

    public ConPlayerFactions()
    {
        FactionsLibrary = Resources.Load<FactionsLibrary>(LibraryLocations.FACTIONS_LIBRARY_LOCATION);
        PlayerFactionLinks = new PlayerFactionLinkItem[Enum.GetNames(typeof(FactionType)).Length - 1];
        FactionType factionType = FactionType.None;
        for(int i = 0; i < PlayerFactionLinks.Length; i++)
        {
            factionType = (FactionType)(i + 1);
            if(factionType != FactionType.None)
                PlayerFactionLinks[i] = new PlayerFactionLinkItem(factionType);
        }
    }

    public void AssignPlayerToFaction(RegisteredPlayer player, FactionType faction)
    {
        for (int i = 0; i < PlayerFactionLinks.Length; i++)
        {
            if (PlayerFactionLinks[i].FactionType == faction)
            {
                PlayerFactionLinks[i].Player = player;
                if(PlayerFactionAssignedEvent != null)
                {
                    PlayerFactionAssignedEvent(PlayerFactionLinks[i]);
                }
                break;
            }
        }
    }

    public void UnassignPlayerFromItsFaction(RegisteredPlayer player)
    {
        for (int i = 0; i < PlayerFactionLinks.Length; i++)
        {
            if (PlayerFactionLinks[i].Player == player)
            {
                PlayerFactionLinks[i].Player = null;
                if(PlayerFactionAssignedEvent != null)
                {
                    PlayerFactionAssignedEvent(PlayerFactionLinks[i]);
                }
                break;
            }
        }
    }

    public FactionType GetFactionTypeOfPlayer(RegisteredPlayer player)
    {
        for (int i = 0; i < PlayerFactionLinks.Length; i++)
        {
            if (PlayerFactionLinks[i].Player == player)
                return PlayerFactionLinks[i].FactionType;
        }
        return FactionType.None;
    }

    public PlayerFactionLinkItem GetLinkItemForFaction(FactionType factionType)
    {
        for (int i = 0; i < PlayerFactionLinks.Length; i++)
        {
            if (PlayerFactionLinks[i].FactionType == factionType)
                return PlayerFactionLinks[i];
        }
        return new PlayerFactionLinkItem(factionType);
    }

    public FactionType[] GetFreeFactions()
    {
        List<FactionType> freeTypes = new List<FactionType>();
        for(int i = 0; i < PlayerFactionLinks.Length; i++)
        {
            if (PlayerFactionLinks[i].Player == null)
                freeTypes.Add(PlayerFactionLinks[i].FactionType);
        }
        return freeTypes.ToArray();
    }

    public FactionType[] GetTakenFactions()
    {
        List<FactionType> takenTypes = new List<FactionType>();
        for (int i = 0; i < PlayerFactionLinks.Length; i++)
        {
            if (PlayerFactionLinks[i].Player != null)
                takenTypes.Add(PlayerFactionLinks[i].FactionType);
        }
        return takenTypes.ToArray();
    }

    public void ConClear()
    {

    }
}

public struct PlayerFactionLinkItem
{
    public RegisteredPlayer Player;
    public FactionType FactionType;

    public PlayerFactionLinkItem(FactionType factionType)
    {
        Player = null;
        FactionType = factionType;
    }
}
