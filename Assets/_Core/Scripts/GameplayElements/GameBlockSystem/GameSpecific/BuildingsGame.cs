using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ramses.Confactory;
using System;
using Ramses.SceneTrackers;
public class BuildingsGame : MonoBehaviour, IGame {

    public GamePlayer[] GamePlayers { get { return _gamePlayers.ToArray(); } }
    private List<GamePlayer> _gamePlayers = new List<GamePlayer>();

    [Header("Game Blocks")]
    [SerializeField]
    private ScriptableGameBlock[] _gameBlocks;

    private BuildingsGameBlockSystem _gameBlockSystem;
    private Playfield _playfield;

    public void EndGameWinCondition()
    {
        _gameBlockSystem.EndBlockCycle();
    }

    public void SpawnBuildGrounds(int amount)
    {
        _playfield.SetCornersBuildfieldsAmount(amount);
    }

    protected void Awake()
    {
        _playfield = SceneTrackersFinder.Instance.GetSceneTracker<PlayfieldST>().Playfield;
        _gameBlockSystem = new BuildingsGameBlockSystem(this, _gameBlocks);
        GenerateGamePlayers();
    }

    protected void Start()
    {
        _gameBlockSystem.StartBlockCycle();
    }

    private void GenerateGamePlayers()
    {
        RegisteredPlayer[] registeredPlayers = ConfactoryFinder.Instance.Get<ConPlayers>().GetRegisteredPlayers();
        for(int i = 0; i < registeredPlayers.Length; i++)
        {
            _gamePlayers.Add(new GamePlayer(registeredPlayers[i]));
        }
    }
}
