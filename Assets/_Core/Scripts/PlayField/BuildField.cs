using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuildField : MonoBehaviour {

	public bool Available { get { return gameObject.activeSelf; } }
    public Building CurrentBuiltBuilding { get; private set; }

    public void ToggleBuildFieldActiveState(bool state)
    {
        if(state != Available)
        {
            gameObject.SetActive(state);
        }
    }

    public void BuildBuilding(Building buildingPrefab)
    {
        DestroyCurrentBuilding();
        CurrentBuiltBuilding = GameObject.Instantiate(buildingPrefab);
        CurrentBuiltBuilding.transform.SetParent(this.transform, false);
    }

    public void DestroyCurrentBuilding()
    {
        if (CurrentBuiltBuilding == null) { return; }
        Destroy(CurrentBuiltBuilding.gameObject);
        CurrentBuiltBuilding = null;
    }
}
