using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuildField : MonoBehaviour {

	public bool Available { get { return gameObject.activeSelf; } }

    public void ToggleBuildFieldActiveState(bool state)
    {
        if(state != Available)
        {
            gameObject.SetActive(state);
        }
    }
}
