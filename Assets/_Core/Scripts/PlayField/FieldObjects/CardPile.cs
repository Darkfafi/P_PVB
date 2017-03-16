using UnityEngine;
using DG.Tweening;

public class CardPile : MonoBehaviour
{
    public delegate void CardDrawInfoHandler(CardDrawInfo cardDrawInfo);
    public CardDrawInfoHandler AllCardsArrivedEvent; 
    public GamePlayerCardHandler CardArrivedToPlayerEvent;

    [SerializeField]
    private GameObject _pileCardPrefab;

    public void DrawCard(CardDrawInfo cardDrawInfo)
    {
        DrawLoop(cardDrawInfo, 1);
    }

    private void DrawLoop(CardDrawInfo cardDrawInfo, int cardCount)
    {
        InternalDrawCard(cardDrawInfo).OnComplete(
        () => {
            if (cardCount < cardDrawInfo.Amount)
            {
                DrawLoop(cardDrawInfo, (cardCount + 1));
            }
            else
            {
                if(AllCardsArrivedEvent != null)
                {
                    AllCardsArrivedEvent(cardDrawInfo);
                }
            }
        });
    }

    private Tweener InternalDrawCard(CardDrawInfo cardDrawInfo)
    {
        BaseCard cardDrawing = PickCardToDraw();
        GameObject pileCard = Instantiate(_pileCardPrefab);
        pileCard.transform.position = transform.position;
        Vector2 destination = Ramses.SceneTrackers.SceneTrackersFinder.Instance.GetSceneTracker<PlayfieldST>().Playfield.GetCornerByFaction(cardDrawInfo.GamePlayer.FactionType).transform.position;
        Vector2 dir = (destination - new Vector2(pileCard.transform.position.x, pileCard.transform.position.y)).normalized;
        destination += (dir * 1.1f);

        pileCard.transform.DOMove(destination, 0.4f).SetEase(Ease.InExpo).OnComplete(
        () =>
        {
            CardArrive(cardDrawInfo.GamePlayer, cardDrawing);
            Destroy(pileCard.gameObject);
        });
        return pileCard.transform.DORotate(new Vector3(0,0, (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) - 90f), 0.2f).SetDelay(0.2f);
    }

    private BaseCard PickCardToDraw()
    {
        BaseCard card;
        ConCards cc = Ramses.Confactory.ConfactoryFinder.Instance.Get<ConCards>();
        GlobalCardDefinitionItem[] cardItems = cc.CardsDefinitionLibrary.GetAllCardDefinitions();
        card = cc.CreateCard(cardItems[Random.Range(0, cardItems.Length)].CardName);
        return card;
    }

    private void CardArrive(GamePlayer gamePlayer, BaseCard card)
    {
        if (CardArrivedToPlayerEvent != null)
            CardArrivedToPlayerEvent(gamePlayer, card);
    }
}

public struct CardDrawInfo
{
    public const string NO_CARD_DEFINED = "<1337> No Card Defined </1337>";

    public int Amount;
    public GamePlayer GamePlayer;
    public string SpecificCard;

    public CardDrawInfo(GamePlayer gamePlayer, int amount)
    {
        Amount = amount;
        GamePlayer = gamePlayer;
        SpecificCard = NO_CARD_DEFINED;
    }

    public CardDrawInfo(GamePlayer gamePlayer, int amount, string specificCard)
    {
        Amount = amount;
        GamePlayer = gamePlayer;
        SpecificCard = specificCard;
    }
}