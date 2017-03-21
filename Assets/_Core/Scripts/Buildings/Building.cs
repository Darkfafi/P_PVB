using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Building : MonoBehaviour {

	public int Score { get; private set; }
    public Skill SkillLinkedTo { get; private set; }

    public void SetBuilding(int score, Skill skill)
    {
        Score = score;
        SkillLinkedTo = skill;
    }
}
