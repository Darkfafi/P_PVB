using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayer
{
    public bool IsConnected { get { return  (_linkedPlayer != null) ? _linkedPlayer.IsConnected : false; } }
    public int PlayerIndex  { get { return  (_linkedPlayer != null) ? _linkedPlayer.PlayerIndex : -1; } }

    public int GoldAmount { get; private set; }
    public List<Card> CardsInHand { get; private set; }

    private RegisteredPlayer _linkedPlayer = null;

	public GamePlayer(RegisteredPlayer linkedPlayer)
    {
        _linkedPlayer = linkedPlayer;
    }
}
