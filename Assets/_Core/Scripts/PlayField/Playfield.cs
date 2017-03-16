using UnityEngine;

public class Playfield : MonoBehaviour
{
    public CardPile CardPile { get { return _cardPile; } }

	[SerializeField]
    private PlayerCorner _spartanCorner;

    [SerializeField]
    private PlayerCorner _knightCorner;

    [SerializeField]
    private PlayerCorner _vikingCorner;

    [SerializeField]
    private PlayerCorner _samuraiCorner;

    [SerializeField]
    private CardPile _cardPile;

    private PlayerCorner[] _allPlayCorners;

    public void SetCornersBuildfieldsAmount(int amount)
    {
        for (int i = 0; i < _allPlayCorners.Length; i++)
        {
            _allPlayCorners[i].SetCornerBuildingSpots(amount);
        }
    }

    public PlayerCorner GetCornerByFaction(FactionType factionType)
    {
        switch (factionType)
        {
            case FactionType.Spartans:
                return _spartanCorner;
            case FactionType.Vikings:
                return _vikingCorner;
            case FactionType.Knights:
                return _knightCorner;
            case FactionType.Samurai:
                return _samuraiCorner;
            default:
                return null;
        }
    }

    protected void Awake()
    {
        _allPlayCorners = new PlayerCorner[] { _spartanCorner, _knightCorner, _vikingCorner, _samuraiCorner };
    }
}
