using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
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
        CurrentBuiltBuilding.GetComponent<SpriteRenderer>().sortingOrder = (GetComponent<SpriteRenderer>().sortingOrder + 1);
        Vector3 pos = CurrentBuiltBuilding.transform.position;
        pos.z = transform.position.y;
        CurrentBuiltBuilding.transform.position = pos;
    }

    public void DestroyCurrentBuilding()
    {
        if (CurrentBuiltBuilding == null) { return; }
        Destroy(CurrentBuiltBuilding.gameObject);
        CurrentBuiltBuilding = null;
    }
}
