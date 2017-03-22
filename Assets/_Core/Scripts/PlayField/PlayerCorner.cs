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

    public BuildField[] GetAllBuildFieldsInUse(Skill skillRelated)
    {
        List<BuildField> bf = new List<BuildField>(GetAllBuildFieldsInUse());

        for(int i = bf.Count - 1; i>=0; i--)
        {
            if (bf[i].CurrentBuiltBuilding.SkillLinkedTo != skillRelated)
                bf.Remove(bf[i]);
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
        ConCards cc = ConfactoryFinder.Instance.Get<ConCards>();
        GlobalCardDefinitionItem cardDefinition = cc.CardsDefinitionLibrary.GetCardDefinitionByName(card.CardName);
        CardDefinitionBaseItem baseCardDefinition = (cc.CardsDefinitionLibrary.GetCardTypeFromCard(card.CardName) == CardType.UpgradeCard) ? cc.CardsDefinitionLibrary.GetCardBaseItemOfUpgradeItem((CardDefinitionUpgradeItem)cardDefinition) : (CardDefinitionBaseItem)cardDefinition;

        for(int i = 0; i < _buildFields.Count; i++)
        {
            if(_buildFields[i].Available && _buildFields[i].CurrentBuiltBuilding == null)
            {
                _buildFields[i].BuildBuilding(cardDefinition, baseCardDefinition);
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
