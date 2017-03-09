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

    private PlayerCorner[] allPlayCorners;

    public void SetCornersBuildfieldsAmount(int amount)
    {
        for (int i = 0; i < allPlayCorners.Length; i++)
        {
            allPlayCorners[i].SetCornerBuildingSpots(amount);
        }
    }

    protected void Awake()
    {
        allPlayCorners = new PlayerCorner[] { _spartanCorner, _knightCorner, _vikingCorner, _samuraiCorner };
    }
}
