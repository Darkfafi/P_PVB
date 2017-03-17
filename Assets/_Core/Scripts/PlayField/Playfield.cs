using UnityEngine;

public class Playfield : MonoBehaviour
{
    public CardPile CardPile { get { return _cardPile; } }
    public CoinPile CoinPile { get { return _coinPile; } }

    public PlayerCorner[] AllPlayCorners { get; private set; }

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

    [SerializeField]
    private CoinPile _coinPile;

    public void SetCornersBuildfieldsAmount(int amount)
    {
        for (int i = 0; i < AllPlayCorners.Length; i++)
        {
            AllPlayCorners[i].SetCornerBuildingSpots(amount);
        }
    }

    public FactionType GetFactionWithHeighestScore()
    {
        PlayerCorner pc = GetPlayerCornerWithHeighestScore();
        if (pc == _spartanCorner)
            return FactionType.Spartans;
        if (pc == _knightCorner)
            return FactionType.Knights;
        if (pc == _samuraiCorner)
            return FactionType.Samurai;
        if (pc == _vikingCorner)
            return FactionType.Vikings;

        return FactionType.None;
    }

    public PlayerCorner GetPlayerCornerWithHeighestScore()
    {
        PlayerCorner pc = AllPlayCorners[0];
        for(int i = 0; i < AllPlayCorners.Length; i++)
        {
            if(AllPlayCorners[i].TotalScoreOfAllBuiltBuildings() > pc.TotalScoreOfAllBuiltBuildings())
            {
                pc = AllPlayCorners[i];
            }
        }
        return pc;
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
        AllPlayCorners = new PlayerCorner[] { _spartanCorner, _knightCorner, _vikingCorner, _samuraiCorner };
    }
}
