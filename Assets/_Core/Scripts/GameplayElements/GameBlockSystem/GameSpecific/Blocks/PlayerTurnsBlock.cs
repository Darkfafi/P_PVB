using System;
using UnityEngine;
using Ramses.SceneTrackers;

public class PlayerTurnsBlock : BaseGameBlock<BuildingsGame, PlayerTurnsBlockInfo, PlayerTurnsBlockLogic>
{
    public override PlayerTurnsBlockInfo BlockInfo
    {
        get
        {
            return info;
        }
    }

    [SerializeField]
    private PlayerTurnsBlockInfo info;
}

[Serializable]
public struct PlayerTurnsBlockInfo : IGameBlockInfo<BuildingsGame>
{
    [SerializeField, Range(10, 120)]
    public int SecondsForEachPlayerTurn;
}

public class PlayerTurnsBlockLogic : BaseGameBlockLogic<BuildingsGame, PlayerTurnsBlockInfo>
{
    private CardInteractionTranslator _cardInteractionTranslator;

    protected override void Initialized()
    {
        _cardInteractionTranslator = SceneTrackersFinder.Instance.GetSceneTracker<AirConsoleMessageST>().Get<CardInteractionTranslator>();
    }

    protected override void Activated()
    {
        _cardInteractionTranslator.CardPlayRequestEvent += OnCardPlayRequestEvent;

        for (int i = 0; i < game.GamePlayers.Length; i++)
        {
            game.GamePlayers[i].PlayCardEvent += OnPlayCardEvent;
        }

        Debug.Log("PlayerTurnsBlock Activated");
    }

    protected override void CycleEnded()
    {

    }

    protected override void CycleStarted()
    {
        Debug.Log("Started PlayerTurnsBlock " + gameBlockInfo.SecondsForEachPlayerTurn);
    }

    protected override void Deactivated()
    {
        _cardInteractionTranslator.CardPlayRequestEvent -= OnCardPlayRequestEvent;

        for (int i = 0; i < game.GamePlayers.Length; i++)
        {
            game.GamePlayers[i].PlayCardEvent -= OnPlayCardEvent;
        }

    }

    protected override void Destroyed()
    {

    }

    private void OnCardPlayRequestEvent(string cardName, int deviceId)
    {
        // TODO: Check if its the requesting player his or her turn.
        GamePlayer p = game.GetGamePlayerByDeviceId(deviceId);
        if (p != null)
        {
            p.PlayCard(cardName);
        }  
    }

    private void OnPlayCardEvent(GamePlayer gamePlayer, BaseCard card)
    {
        _cardInteractionTranslator.SendAllowedPlayRequest(true, "Playing Card: " + card.CardName, gamePlayer.LinkedPlayer.DeviceID, card.CardName);
        _cardInteractionTranslator.SendUpdateCardsShown(gamePlayer.LinkedPlayer.DeviceID, gamePlayer.GetNameListOfCardsInHand());
        game.Playfield.GetCornerByFaction(gamePlayer.FactionType).BuildStructureForCard(card);
    }
}
