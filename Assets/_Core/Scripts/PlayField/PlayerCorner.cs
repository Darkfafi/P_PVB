using System.Collections.Generic;
using UnityEngine;
using Ramses.Confactory;

public class PlayerCorner : MonoBehaviour
{
    private List<BuildField> _buildFields = new List<BuildField>();

    public BuildField[] GetAllBuildFieldsInUse()
    {
        List<BuildField> bf = new List<BuildField>();
        for(int i = 0; i < _buildFields.Count; i++)
        {
            if(_buildFields[i].CurrentBuiltBuilding != null)
            {
                bf.Add(_buildFields[i]);
            }
        }

        return bf.ToArray();
    }

    public int TotalScoreOfAllBuiltBuildings()
    {
        BuildField[] usedBuildfields = GetAllBuildFieldsInUse();
        int totalScore = 0;
        for (int i = 0; i < usedBuildfields.Length; i++)
        {
            totalScore += usedBuildfields[i].CurrentBuiltBuilding.Score;
        }
        return totalScore;
    }

    public void BuildStructureForCard(BaseCard card)
    {
        Building buildingPrefab = ConfactoryFinder.Instance.Get<ConCards>().CardsDefinitionLibrary.GetCardDefinitionByName(card.CardName).CardBuildingObjectPrefab;
        for(int i = 0; i < _buildFields.Count; i++)
        {
            if(_buildFields[i].Available && _buildFields[i].CurrentBuiltBuilding == null)
            {
                _buildFields[i].BuildBuilding(buildingPrefab);
                break;
            }
        }
    }

    public bool CanBuildStructureForCard(BaseCard card)
    {
        return true;
    }

    public bool HasStructureForCard(BaseCard card)
    {
        return true;
    }

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
