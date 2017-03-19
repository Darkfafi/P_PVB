using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spintest : MonoBehaviour {

    [SerializeField]
    private FortuneWheelPopUp popUp;

	// Use this for initialization
	void Start ()
    {
        PopUpSystem.Instance.CreatePopUp(popUp).Spin(new FactionType[] { FactionType.Knights, FactionType.Samurai, FactionType.Vikings}, 0);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
