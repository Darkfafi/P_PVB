using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ramses.Confactory;
using System;
using Ramses.SceneTrackers;

/// <summary>
/// This class can be seen as the 'TableBoard' of the entire game. It has info on who is playing, what is on the field, and the rules to follow.
/// </summary>
public class BuildingsGame : MonoBehaviour, IGame
{
    /// <summary>
    /// All the players seated to the game table
    /// </summary>
    public GamePlayer[] GamePlayers { get { return _gamePlayers.ToArray(); } }
    private List<GamePlayer> _gamePlayers = new List<GamePlayer>();

    /// <summary>
    /// The playfield which is played on.
    /// </summary>
    public Playfield Playfield { get{ return _playfield; } }
    private Playfield _playfield;

    /// <summary>
    /// The amount of cards the players start with. (The amount they grab at the start of the game)
    /// </summary>
    public int StartHandCardAmount { get { return _startHandCardAmount; } }
    /// <summary>
    /// The amount of coins the players start with. (The amount they grab at the start of the game)
    /// </summary>
    public int StartGoldAmount { get { return _startGoldAmount; } }

    [Header("Global Game Options")]
    [SerializeField]
    private int _startHandCardAmount = 4;

    [SerializeField]
    private int _startGoldAmount = 2;

    [Header("Game Blocks")]
    [SerializeField]
    private ScriptableGameBlock[] _gameBlocks;

    private BuildingsGameBlockSystem _gameBlockSystem;

    /// <summary>
    /// This method ends the game and asks for which player has won the game when ended.
    /// </summary>
    /// <param name="winner">The winner of the game which was played</param>
    public void EndGameWinCondition(GamePlayer winner)
    {
        _gameBlockSystem.EndBlockCycle();
    }

    /// <summary>
    /// This spawns the amount of buildfields which must be active during the game.
    /// </summary>
    /// <param name="amount"></param>
    public void SpawnBuildGrounds(int amount)
    {
        _playfield.SetCornersBuildfieldsAmount(amount);
    }

    /// <summary>
    /// Returns a GamePlayer by its linked RegisteredPlayer
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public GamePlayer GetGamePlayerBy(RegisteredPlayer player)
    {
        return GetGamePlayerByDeviceId(player.DeviceID);
    }

    /// <summary>
    /// Returns a GamePlayer by its FactionType
    /// </summary>
    /// <param name="factionType"></param>
    /// <returns></returns>
    public GamePlayer GetGamePlayerBy(FactionType factionType)
    {
        for (int i = 0; i < GamePlayers.Length; i++)
        {
            if (GamePlayers[i].FactionType == factionType)
                return GamePlayers[i];
        }
        return null;
    }

    /// <summary>
    /// Returns a GamePlayer bt its PlayerIndex
    /// </summary>
    /// <param name="playerIndex"></param>
    /// <returns></returns>
    public GamePlayer GetGamePlayerBy(int playerIndex)
    {
        for(int i = 0; i < GamePlayers.Length; i++)
        {
            if (GamePlayers[i].PlayerIndex == playerIndex)
                return GamePlayers[i];
        }
        return null;
    }

    /// <summary>
    /// Returns a GamePlayer by its currently linked Device(id)
    /// </summary>
    /// <param name="deviceId"></param>
    /// <returns></returns>
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
        GenerateGamePlayers();
        _gameBlockSystem = new BuildingsGameBlockSystem(this, _gameBlocks);
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
        PlayersStarterGift();
    }

    private void StartGame()
    {
        _gameBlockSystem.StartBlockCycle();
    }

    private void PlayersStarterGift()
    {
        if (GamePlayers.Length == 0) { Debug.LogError("NO PLAYERS DETECTED. PLEASE START FROM LOBBY SCENE!"); return; }
        for(int i = 0; i < GamePlayers.Length; i++)
        {
            GamePlayers[i].AllRequestedCardsReceivedEvent -= OnAllRequestedCardsReceivedEvent;
            GamePlayers[i].AllRequestedCardsReceivedEvent += OnAllRequestedCardsReceivedEvent;

            GamePlayers[i].GrabCoins(_startGoldAmount);
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
