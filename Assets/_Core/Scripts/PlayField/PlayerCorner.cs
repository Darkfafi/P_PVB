using System.Collections.Generic;
using UnityEngine;
using Ramses.Confactory;

/// <summary>
/// This represents the corner of a player.
/// </summary>
public class PlayerCorner : MonoBehaviour
{
    private List<BuildField> _buildFields = new List<BuildField>();

    /// <summary>
    /// Returns all the buildfields with buildings built on them.
    /// </summary>
    /// <returns>The buildfields with a building</returns>
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

    /// <summary>
    /// Returns all the buildfields with buildings built with the related skill type given.
    /// </summary>
    /// <param name="skillRelated">The related skill type for the buildings</param>
    /// <returns>All the buildfields with buildings which are related to the skill given</returns>
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
