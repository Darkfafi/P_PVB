using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour {

	public int Score { get; private set; }

    public void SetScore(int value)
    {
        Score = value;
    }
}
