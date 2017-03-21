using System;
using UnityEngine;
using Ramses.SceneTrackers;

public class GamePlayersUI : MonoBehaviour {

    [SerializeField]
    private GamePlayerSectionUI _knightSection;

    [SerializeField]
    private GamePlayerSectionUI _vikingSection;

    [SerializeField]
    private GamePlayerSectionUI _spartanSection;

    [SerializeField]
    private GamePlayerSectionUI _samuraiSection;

    private BuildingsGameST _buildingsGameSceneTracker;

    protected void Awake()
    {
        _buildingsGameSceneTracker = SceneTrackersFinder.Instance.GetSceneTracker<BuildingsGameST>();
    }

    protected void Start()
    {
        AssignGamePlayersToUI();
    }

    private void AssignGamePlayersToUI()
    {
        foreach(FactionType ft in Enum.GetValues(typeof(FactionType)))
        {
            GamePlayerSectionUI section = null;
            switch (ft)
            {
                case FactionType.Spartans:
                    section = _spartanSection;
                    break;
                case FactionType.Vikings:
                    section = _vikingSection;
                    break;
                case FactionType.Knights:
                    section = _knightSection;
                    break;
                case FactionType.Samurai:
                    section = _samuraiSection;
                    break;
                default:
                    continue;
            }
            PlayerFactionLinkItem pfli = Ramses.Confactory.ConfactoryFinder.Instance.Get<ConPlayerFactions>().GetLinkItemForFaction(ft);
            if (pfli.Player != null)
                section.DisplayGamePlayer(_buildingsGameSceneTracker.BuildingsGame.GetGamePlayerBy(pfli.Player));
            else
            {
                section.SetFactionSpecifics(ft);
                section.ToggleActivePlayerDisplay(false);
            }
        }
    }
}
