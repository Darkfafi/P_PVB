using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void GamePlayerSkillHandler(GamePlayer player, Skill skill);

public class SkillEffects
{
    public event GamePlayerSkillHandler SkillEffectDoneEvent;
    private BuildingsGame _game;

    private FactionType _effectedFactionType;
    private GamePlayer _currentEffectGamePlayer;
    private Skill _currentEffectOfSkill;
    private FortuneWheelPopUp _fortuneWheelPopUp = null;
    private bool _inEffect = false;

    public SkillEffects(BuildingsGame buildingsGame)
    {
        _game = buildingsGame;
    }

    public void Destroy()
    {
        DoneEffect(_currentEffectGamePlayer, _currentEffectOfSkill);
    }

    public bool DoSkillEffect(GamePlayer gamePlayer, Skill skill)
    {
        if (_inEffect) { return false; }
        _inEffect = true;
        _currentEffectOfSkill = skill;
        _currentEffectGamePlayer = gamePlayer;
        FactionType[] takenTypes = Ramses.Confactory.ConfactoryFinder.Instance.Get<ConPlayerFactions>().GetTakenFactions();
        int index = UnityEngine.Random.Range(0, takenTypes.Length);
        switch (skill)
        {
            case Skill.Miracle:
                // Do nothing because only passive skill (Not effected by destruction)
                DoneEffect(gamePlayer, skill);
                break;
            case Skill.Trade:
                // Give player coins
                break;
            case Skill.Destruction:
                // Destroyer call for random building of random player
                _fortuneWheelPopUp = PopUpSystem.Instance.CreatePopUp<FortuneWheelPopUp>("FortuneWheel");
                _effectedFactionType = takenTypes[index];
                _fortuneWheelPopUp.Spin(takenTypes, index);

                _fortuneWheelPopUp.PopUpBeingDestroyedEvent -= OnDestructionEffectEnd;
                _fortuneWheelPopUp.PopUpBeingDestroyedEvent += OnDestructionEffectEnd;
                break;
            case Skill.Thief:
                // Stealing call for all gold of random player
                _fortuneWheelPopUp = PopUpSystem.Instance.CreatePopUp<FortuneWheelPopUp>("FortuneWheel");
                _effectedFactionType = takenTypes[index];
                _fortuneWheelPopUp.Spin(takenTypes, index);

                _fortuneWheelPopUp.PopUpBeingDestroyedEvent -= OnThiefEffectEnd;
                _fortuneWheelPopUp.PopUpBeingDestroyedEvent += OnThiefEffectEnd;
                break;
            case Skill.TheCrown:
                // Do nothing because only passive skill (gaining money and starting first)
                DoneEffect(gamePlayer, skill);
                break;
            default:
                Debug.LogError("<< Unknown skill used: " + skill.ToString());
                break;
        }
        return true;
    }

    private void OnDestructionEffectEnd(BasePopUp popUpEffected)
    {
        popUpEffected.PopUpBeingDestroyedEvent -= OnDestructionEffectEnd;
        BuildField[] buildFieldsOfTargetFaction = _game.Playfield.GetCornerByFaction(_effectedFactionType).GetAllBuildFieldsInUse();
        if (buildFieldsOfTargetFaction.Length > 0)
            buildFieldsOfTargetFaction[UnityEngine.Random.Range(0, buildFieldsOfTargetFaction.Length)].DestroyCurrentBuilding();

        DoneEffect(_currentEffectGamePlayer, _currentEffectOfSkill);
    }

    private void OnThiefEffectEnd(BasePopUp popUpEffected)
    {
        popUpEffected.PopUpBeingDestroyedEvent -= OnThiefEffectEnd;
        _currentEffectGamePlayer.GiveCoins(_game.GetGamePlayerBy(_effectedFactionType).TakeCoins());

        DoneEffect(_currentEffectGamePlayer, _currentEffectOfSkill);
    }

    private void DoneEffect(GamePlayer player, Skill skill)
    {
        if (!_inEffect) { return; }
        _inEffect = false;
        _currentEffectGamePlayer = null;
        _currentEffectOfSkill = Skill.None;
        _effectedFactionType = FactionType.None;

        if(_fortuneWheelPopUp != null)
        {
            _fortuneWheelPopUp.PopUpBeingDestroyedEvent -= OnDestructionEffectEnd;
            _fortuneWheelPopUp.PopUpBeingDestroyedEvent -= OnThiefEffectEnd;
        }

        if (SkillEffectDoneEvent != null)
            SkillEffectDoneEvent(player, skill);
    }
}
