
public enum Skill
{
    None, 
    Miracle,        // Can't be effected by destruction                     :: Passive (System thingy?..)
    Trade,          // Gains Money on activation                            :: Active  (On activate do automaticly)
    Destruction,    // Destroys a building by random                        :: Active  (On activate do automaticly)
    Thief,          // Steals money from a person by random                 :: Active  (On activate do automaticly)
    TheCrown        // Starts as first & gains money from all buildings     :: Passive (Start round and starts first (System thingy))
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
        if (_isUsed) { return; }
        _isUsed = true;
        if (SkillUsedEvent != null)
            SkillUsedEvent(SkillPouchHolder, Skill);
    }

    public void SetSkill(Skill skill)
    {
        Skill = skill;
        _isUsed = false;
        if (SkillSetEvent != null)
            SkillSetEvent(SkillPouchHolder, Skill);
    }
}

public interface ICharacterSkill
{
    bool CanBeActivated { get; }
    void Activate();
}