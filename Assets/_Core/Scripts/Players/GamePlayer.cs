using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void GamePlayerCardHandler(GamePlayer gamePlayer, BaseCard card);
public delegate void GamePlayerHandler(GamePlayer gamePlayer);

/// <summary>
/// This is the 'game version' of the registered player. The GamePlayer contains the player his or her stats which are linked to the gameplay
/// </summary>
public class GamePlayer
{
    /// <summary>
    /// This event will be triggered when the player has given the card to the game to do its effect with it.
    /// </summary>
    public event GamePlayerCardHandler PlayCardEvent;
    /// <summary>
    /// This event will be triggered when the player has received a card from the CardPile
    /// </summary>
    public event GamePlayerCardHandler ReceivedCardEvent;
    /// <summary>
    /// This event will be triggered when the player has reveiced all the requested cards from the CardPile
    /// </summary>
    public event GamePlayerHandler AllRequestedCardsReceivedEvent;

    /// <summary>
    /// The faction this player has been assigned to.
    /// </summary>
    public FactionType FactionType {
        get
        {
            return Ramses.Confactory.ConfactoryFinder.Instance.Get<ConPlayerFactions>().GetFactionTypeOfPlayer(LinkedPlayer);
        }
    }

    /// <summary>
    /// The linked registered player to the game player
    /// </summary>
    public RegisteredPlayer LinkedPlayer { get { return _linkedPlayer; } }

    /// <summary>
    /// This returns the 'IsConnected' status of the 'LinkedPlayer' variable in the same class.
    /// </summary>
    public bool IsConnected { get { return  (_linkedPlayer != null) ? _linkedPlayer.IsConnected : false; } }
    /// <summary>
    /// This returns the 'PlayerIndex' value of the 'LinkedPlayer' variable in the same class.
    /// </summary>
    public int PlayerIndex  { get { return  (_linkedPlayer != null) ? _linkedPlayer.PlayerIndex : -1; } }

    /// <summary>
    /// This is the amount of gold the player has.
    /// </summary>
    public int GoldAmount { get; private set; }

    /// <summary>
    /// These are the cards the player has in his 'hand'
    /// </summary>
    public BaseCard[] CardsInHand { get { return _cardsInHand.ToArray(); } }

    /// <summary>
    /// This holds the information of the skill the player has in his possetion
    /// </summary>
    public SkillPouch SkillPouch { get; private set; }

    private List<BaseCard> _cardsInHand = new List<BaseCard>();
    private RegisteredPlayer _linkedPlayer = null;

    private PlayfieldST _playfieldSceneTracker;

    /// <summary>
    /// To create a GamePlayer, there must be a RegisteredPlayer linked to it. This player will not change as long as the GamePlayer lives.
    /// </summary>
    /// <param name="linkedPlayer">Registered player (The seat) for the GamePlayer</param>
    public GamePlayer(RegisteredPlayer linkedPlayer)
    {
        _linkedPlayer = linkedPlayer;
        SkillPouch = new SkillPouch(this);
        _playfieldSceneTracker = Ramses.SceneTrackers.SceneTrackersFinder.Instance.GetSceneTracker<PlayfieldST>();

        _playfieldSceneTracker.Playfield.CoinPile.VisualObjectArrivedEvent += OnCoinArrivedToPlayerEvent;

        _playfieldSceneTracker.Playfield.CardPile.VisualObjectArrivedEvent += OnCardArrivedToPlayerEvent;
        _playfieldSceneTracker.Playfield.CardPile.AllObjectsArrivedEvent += OnAllCardsArrivedEvent;
    }

    /// <summary>
    /// Gives a string array of all the card names in the hand of the player.
    /// </summary>
    /// <returns></returns>
    public string[] GetNameListOfCardsInHand()
    {
        string[] names = new string[_cardsInHand.Count];
        for(int i = 0; i < _cardsInHand.Count; i++)
        {
            names[i] = _cardsInHand[i].CardName;
        }
        return names;
    }

    /// <summary>
    /// Gives coins to the player
    /// </summary>
    /// <param name="amount">amount of coins to give to the player</param>
    public void GiveCoins(int amount)
    {
        GoldAmount += amount;
    }

    /// <summary>
    /// Takes all the coins away from the player
    /// </summary>
    /// <returns></returns>
    public int TakeCoins()
    {
        return TakeCoins(GoldAmount);
    }


    /// <summary>
    /// Takes a x amount of coins away from the player
    /// </summary>
    /// <param name="amount">Amount of coinst to take from the player</param>
    /// <returns></returns>
    public int TakeCoins(int amount)
    {
        amount = Mathf.Clamp(amount, 0, GoldAmount);
        GoldAmount -= amount;
        return amount;
    }

    /// <summary>
    /// Makes the player grab the coins from the CoinPile. This will trigger the coin giving effect on the gameboard.
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool GrabCoins(int amount)
    {
        _playfieldSceneTracker.Playfield.CoinPile.Grab(this, amount);
        return true;
    }

    /// <summary>
    /// Makes the player grab a card from the CardPile. This will trigger the card giving effect on the gameboard.
    /// </summary>
    /// <returns></returns>
    public bool DrawCard()
    {
        _playfieldSceneTracker.Playfield.CardPile.Grab(this, 1);
        return true;
    }

    /// <summary>
    /// Makes the player grab x amount of cards from the CardPile. This will trigger the card giving effect on the gameboard.
    /// </summary>
    /// <param name="amount">The amount of cards to grab</param>
    /// <returns></returns>
    public bool DrawCard(int amount)
    {
        _playfieldSceneTracker.Playfield.CardPile.Grab(this, amount);
        return true;
    }

    /// <summary>
    /// This will play a card from his hard on the game if able.
    /// This means if the card is playable but also if the card is in the player his r her hand.
    /// </summary>
    /// <param name="cardName">Returns if the player has played the card. If false, the player has not played the card.</param>
    /// <returns></returns>
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

    /// <summary>
    /// Makes the GamePlayer ready to be destroyed.
    /// </summary>
    public void Destroy()
    {
        _playfieldSceneTracker.Playfield.CoinPile.VisualObjectArrivedEvent -= OnCoinArrivedToPlayerEvent;
        _playfieldSceneTracker.Playfield.CardPile.VisualObjectArrivedEvent -= OnCardArrivedToPlayerEvent;
        _playfieldSceneTracker.Playfield.CardPile.AllObjectsArrivedEvent -= OnAllCardsArrivedEvent;
    }

    private void OnCoinArrivedToPlayerEvent(GamePlayer player, CoinData objectGrabbed)
    {
        if (player != this) { return; }
        GiveCoins(1);
    }

    private void OnAllCardsArrivedEvent(GamePlayer player)
    {
        if (player == this)
            if (AllRequestedCardsReceivedEvent != null)
                AllRequestedCardsReceivedEvent(player);
    }

    private void OnCardArrivedToPlayerEvent(GamePlayer gamePlayer, BaseCard card)
    {
        if (gamePlayer != this) { return; }

        _cardsInHand.Add(card);

        if (ReceivedCardEvent != null)
            ReceivedCardEvent(this, card);
    }
}
