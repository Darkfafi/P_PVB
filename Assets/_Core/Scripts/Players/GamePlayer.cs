using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void GamePlayerCardHandler(GamePlayer gamePlayer, BaseCard card);

public class GamePlayer
{
    public event GamePlayerCardHandler PlayCardEvent;

    public FactionType FactionType {
        get
        {
            return Ramses.Confactory.ConfactoryFinder.Instance.Get<ConPlayerFactions>().GetFactionTypeOfPlayer(LinkedPlayer);
        }
    }
    public RegisteredPlayer LinkedPlayer { get { return _linkedPlayer; } }

    public bool IsConnected { get { return  (_linkedPlayer != null) ? _linkedPlayer.IsConnected : false; } }
    public int PlayerIndex  { get { return  (_linkedPlayer != null) ? _linkedPlayer.PlayerIndex : -1; } }

    public int GoldAmount { get; private set; }
    public List<BaseCard> CardsInHand { get; private set; }

    private void OnTurnGained()
    {
        
    }

    private RegisteredPlayer _linkedPlayer = null;

	public GamePlayer(RegisteredPlayer linkedPlayer)
    {
        CardsInHand = new List<BaseCard>();
        _linkedPlayer = linkedPlayer;
    }

    public string[] GetNameListOfCardsInHand()
    {
        string[] names = new string[CardsInHand.Count];
        for(int i = 0; i < CardsInHand.Count; i++)
        {
            names[i] = CardsInHand[i].CardName;
        }
        return names;
    }

    public void DrawCard()
    {
        //TODO: Ask the cardpile for random card
        CardsInHand.Add(Ramses.Confactory.ConfactoryFinder.Instance.Get<ConCards>().CreateCard("HouseOne"));
        CardsInHand.Add(Ramses.Confactory.ConfactoryFinder.Instance.Get<ConCards>().CreateCard("HouseTwo"));
        Debug.Log(CardsInHand.Count);
    }

    public void DrawCard(CardType cardType)
    {
        //TODO: Ask the cardpile for random card of type
    }

    public void DrawCard(string cardName)
    {
        //TODO Ask the cardpile for specific card
    }

    public bool PlayCard(string cardName)
    {
        for(int i = CardsInHand.Count - 1; i >= 0; i--)
        {
            if(CardsInHand[i].CardName == cardName)
            {
                if (CardsInHand[i].IsPlayable(FactionType))
                {
                    if(PlayCardEvent != null)
                    {
                        PlayCardEvent(this, CardsInHand[i]);
                    }
                    CardsInHand.RemoveAt(i);
                    return true;
                }
            }
        }
        return false;
    }
}
