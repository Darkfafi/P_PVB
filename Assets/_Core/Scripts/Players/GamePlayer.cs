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
    public BaseCard[] CardsInHand { get { return _cardsInHand.ToArray(); } }

    private List<BaseCard> _cardsInHand = new List<BaseCard>();

    private void OnTurnGained()
    {
        
    }

    private RegisteredPlayer _linkedPlayer = null;

	public GamePlayer(RegisteredPlayer linkedPlayer)
    {
        _linkedPlayer = linkedPlayer;
    }

    public string[] GetNameListOfCardsInHand()
    {
        string[] names = new string[_cardsInHand.Count];
        for(int i = 0; i < _cardsInHand.Count; i++)
        {
            names[i] = _cardsInHand[i].CardName;
        }
        return names;
    }

    public void DrawCard()
    {
        //TODO: Ask the cardpile for random card
        _cardsInHand.Add(Ramses.Confactory.ConfactoryFinder.Instance.Get<ConCards>().CreateCard("HouseOne"));
        _cardsInHand.Add(Ramses.Confactory.ConfactoryFinder.Instance.Get<ConCards>().CreateCard("HouseTwo"));
        Debug.Log(_cardsInHand.Count);
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
        for(int i = _cardsInHand.Count - 1; i >= 0; i--)
        {
            if(_cardsInHand[i].CardName == cardName)
            {
                if (_cardsInHand[i].IsPlayable(FactionType))
                {
                    if(PlayCardEvent != null)
                    {
                        PlayCardEvent(this, _cardsInHand[i]);
                    }
                    _cardsInHand.RemoveAt(i);
                    return true;
                }
            }
        }
        return false;
    }
}
