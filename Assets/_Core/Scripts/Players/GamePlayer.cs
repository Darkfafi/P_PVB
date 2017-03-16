﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void GamePlayerCardHandler(GamePlayer gamePlayer, BaseCard card);
public delegate void CardDrawInfoHandler(CardDrawInfo info);

public class GamePlayer
{
    public event GamePlayerCardHandler PlayCardEvent;
    public event GamePlayerCardHandler ReceivedCardEvent;
    public event CardDrawInfoHandler AllRequestedCardsReceivedEvent;

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
    private RegisteredPlayer _linkedPlayer = null;

    private PlayfieldST _playfieldSceneTracker;

    public GamePlayer(RegisteredPlayer linkedPlayer)
    {
        _linkedPlayer = linkedPlayer;
        _playfieldSceneTracker = Ramses.SceneTrackers.SceneTrackersFinder.Instance.GetSceneTracker<PlayfieldST>();

        _playfieldSceneTracker.Playfield.CardPile.CardArrivedToPlayerEvent += OnCardArrivedToPlayerEvent;
        _playfieldSceneTracker.Playfield.CardPile.AllCardsArrivedEvent += OnAllCardsArrivedEvent;
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

    public bool DrawCard()
    {
        _playfieldSceneTracker.Playfield.CardPile.DrawCard(new CardDrawInfo(this, 1));
        return true;
    }

    public bool DrawCard(int amount)
    {
        _playfieldSceneTracker.Playfield.CardPile.DrawCard(new CardDrawInfo(this, amount));
        return true;
    }

    public bool DrawCard(string cardName)
    {
        _playfieldSceneTracker.Playfield.CardPile.DrawCard(new CardDrawInfo(this, 1, cardName));
        return true;
    }

    public bool PlayCard(string cardName)
    {
        for(int i = _cardsInHand.Count - 1; i >= 0; i--)
        {
            if(_cardsInHand[i].CardName == cardName)
            {
                if (_cardsInHand[i].IsPlayable(FactionType) && GoldAmount >= _cardsInHand[i].CardCost)
                {
                    GoldAmount -= _cardsInHand[i].CardCost;
                    BaseCard card = _cardsInHand[i];
                    _cardsInHand.RemoveAt(i);

                    if (PlayCardEvent != null)
                    {
                        PlayCardEvent(this, card);
                    }
                    return true;
                }
            }
        }
        return false;
    }

    public void Destroy()
    {
        _playfieldSceneTracker.Playfield.CardPile.CardArrivedToPlayerEvent -= OnCardArrivedToPlayerEvent;
        _playfieldSceneTracker.Playfield.CardPile.AllCardsArrivedEvent -= OnAllCardsArrivedEvent;
    }

    private void OnAllCardsArrivedEvent(CardDrawInfo cardDrawInfo)
    {
        if (cardDrawInfo.GamePlayer == this)
            if (AllRequestedCardsReceivedEvent != null)
                AllRequestedCardsReceivedEvent(cardDrawInfo);
    }

    private void OnCardArrivedToPlayerEvent(GamePlayer gamePlayer, BaseCard card)
    {
        if (gamePlayer != this) { return; }

        _cardsInHand.Add(card);

        if (ReceivedCardEvent != null)
            ReceivedCardEvent(this, card);
    }
}
