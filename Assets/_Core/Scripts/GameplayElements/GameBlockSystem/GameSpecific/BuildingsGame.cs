using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ramses.Confactory;
using System;
using Ramses.SceneTrackers;
public class BuildingsGame : MonoBehaviour, IGame {

    public GamePlayer[] GamePlayers { get { return _gamePlayers.ToArray(); } }
    private List<GamePlayer> _gamePlayers = new List<GamePlayer>();

    public Playfield Playfield { get{ return _playfield; } }
    private Playfield _playfield;

    public int StartHandCardAmount { get { return _startHandCardAmount; } }

    [Header("Global Game Options")]
    [SerializeField]
    private int _startHandCardAmount = 4;

    [Header("Game Blocks")]
    [SerializeField]
    private ScriptableGameBlock[] _gameBlocks;

    private BuildingsGameBlockSystem _gameBlockSystem;

    public void EndGameWinCondition(GamePlayer winner)
    {
        _gameBlockSystem.EndBlockCycle();
    }

    public void SpawnBuildGrounds(int amount)
    {
        _playfield.SetCornersBuildfieldsAmount(amount);
    }

    public GamePlayer GetGamePlayerBy(RegisteredPlayer player)
    {
        return GetGamePlayerByDeviceId(player.DeviceID);
    }

    public GamePlayer GetGamePlayerBy(int playerIndex)
    {
        for(int i = 0; i < GamePlayers.Length; i++)
        {
            if (GamePlayers[i].PlayerIndex == playerIndex)
                return GamePlayers[i];
        }
        return null;
    }

    public GamePlayer GetGamePlayerByDeviceId(int deviceId)
    {
        for(int i = _gamePlayers.Count - 1; i >=0; i--)
        {
            if (_gamePlayers[i].LinkedPlayer.DeviceID == deviceId)
                return _gamePlayers[i];
        }
        return null;
    }

    protected void Awake()
    {
        _playfield = SceneTrackersFinder.Instance.GetSceneTracker<PlayfieldST>().Playfield;
        _gameBlockSystem = new BuildingsGameBlockSystem(this, _gameBlocks);
        GenerateGamePlayers();
    }

    protected void Start()
    {
        Introduction();
    }

    protected void OnDestroy()
    {
        for(int i = GamePlayers.Length - 1; i >= 0; i--)
        {
            GamePlayers[i].AllRequestedCardsReceivedEvent -= OnAllRequestedCardsReceivedEvent;
            GamePlayers[i].Destroy();
        }
    }

    private void Introduction()
    {
        PlayersHandDraw();
    }

    private void StartGame()
    {
        _gameBlockSystem.StartBlockCycle();
    }

    private void PlayersHandDraw()
    {
        if (GamePlayers.Length == 0) { Debug.LogError("NO PLAYERS DETECTED. PLEASE START FROM LOBBY SCENE!"); return; }
        for(int i = 0; i < GamePlayers.Length; i++)
        {
            GamePlayers[i].AllRequestedCardsReceivedEvent -= OnAllRequestedCardsReceivedEvent;
            GamePlayers[i].AllRequestedCardsReceivedEvent += OnAllRequestedCardsReceivedEvent;
        }
        StartCoroutine(PlayersHandDrawLoop(0, 0));
    }

    private IEnumerator PlayersHandDrawLoop(int index, float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);

        if (index < GamePlayers.Length)
            GamePlayers[index].DrawCard(StartHandCardAmount);
        else
            StartGame();
    }

    private void OnAllRequestedCardsReceivedEvent(GamePlayer player)
    {
        player.AllRequestedCardsReceivedEvent -= OnAllRequestedCardsReceivedEvent;
        StartCoroutine(PlayersHandDrawLoop((GamePlayers.GetIndexOf(player) + 1), 0.08f));
    }

    private void GenerateGamePlayers()
    {
        RegisteredPlayer[] registeredPlayers = ConfactoryFinder.Instance.Get<ConPlayers>().GetCurrentlyRegisteredPlayers(false);
        GamePlayer gamePlayer = null;
        for (int i = 0; i < registeredPlayers.Length; i++)
        {
            gamePlayer = new GamePlayer(registeredPlayers[i]);
            _gamePlayers.Add(gamePlayer);
        }
    }
}
