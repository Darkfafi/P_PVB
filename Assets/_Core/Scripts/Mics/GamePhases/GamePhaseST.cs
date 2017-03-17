using UnityEngine;
using Ramses.SceneTrackers;

public class GamePhaseST : MonoBehaviour, ISceneTracker {

    [SerializeField]
    private GamePhase _gamePhaseOnSceneStart;

    public void SystemAwakeCall()
    {
        Ramses.Confactory.ConfactoryFinder.Instance.Get<ConCurrentPhase>().SetCurrentPhase(_gamePhaseOnSceneStart);
    }

    public void SystemDestroyCall()
    {

    }

    public void SystemRequested()
    {

    }

    public void SystemStartCall()
    {

    }
}
