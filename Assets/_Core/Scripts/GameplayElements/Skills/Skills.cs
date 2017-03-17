
public enum Skill
{
    None,
    Miracle,
    Trade,
    Destruction,
    Thief,
    TheCrown   
}

public class SkillPouch
{
    public delegate void SkillGamePlayerHandler(GamePlayer gamePlayer, Skill skill);
    public event SkillGamePlayerHandler SkillSetEvent;
    public event SkillGamePlayerHandler SkillUsedEvent;

    public Skill Skill { get; private set; }
    public bool IsUsed { get { return _isUsed; } }

    private bool _isUsed = false;

    public GamePlayer SkillPouchHolder { get; private set; }

    public SkillPouch(GamePlayer player)
    {
        SkillPouchHolder = player;
    }

    public void UseSkill()
    {
        _isUsed = true;
        if (SkillUsedEvent != null)
            SkillUsedEvent(SkillPouchHolder, Skill);
    }

    public void SetSkill(Skill skill)
    {
        Skill = Skill;
        _isUsed = false;
        if (SkillSetEvent != null)
            SkillSetEvent(SkillPouchHolder, Skill);
    }
}

public interface ICharacterSkill
{

}