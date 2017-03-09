using System.Collections.Generic;
using UnityEngine;

public class PlayerCorner : MonoBehaviour
{
    private List<BuildField> _buildFields = new List<BuildField>();


	public void SetCornerBuildingSpots(int amount)
    {
        for(int i = 0; i < _buildFields.Count; i++)
        {
            _buildFields[i].ToggleBuildFieldActiveState((i < amount));
        }
    }

    protected void Awake()
    {
        _buildFields.AddRange(gameObject.GetComponentsInChildren<BuildField>());
    }
}
