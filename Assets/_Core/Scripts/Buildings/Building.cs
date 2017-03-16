using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Building : MonoBehaviour {

	public int Score { get; private set; }

    public void SetScore(int value)
    {
        Score = value;
    }
}
