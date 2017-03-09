using UnityEngine;

public class Playfield : MonoBehaviour {

	[SerializeField]
    private PlayerCorner _spartanCorner;

    [SerializeField]
    private PlayerCorner _knightCorner;

    [SerializeField]
    private PlayerCorner _vikingCorner;

    [SerializeField]
    private PlayerCorner _samuraiCorner;

    private PlayerCorner[] _allPlayCorners;

    public void SetCornersBuildfieldsAmount(int amount)
    {
        for (int i = 0; i < _allPlayCorners.Length; i++)
        {
            _allPlayCorners[i].SetCornerBuildingSpots(amount);
        }
    }

    protected void Awake()
    {
        _allPlayCorners = new PlayerCorner[] { _spartanCorner, _knightCorner, _vikingCorner, _samuraiCorner };
    }
}
