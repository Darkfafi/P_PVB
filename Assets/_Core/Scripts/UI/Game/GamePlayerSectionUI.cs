using NDream.AirConsole;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ramses.SceneTrackers;

[RequireComponent(typeof(CanvasGroup))]
public class GamePlayerSectionUI : MonoBehaviour
{
    public bool IsActiveDisplay { get; private set; }

    [Header("Options")]
    [SerializeField, Range(0.1f, 0.9f)]
    private float _inactiveDisplayAlpha;

    [Header("Global Requirements")]
    [SerializeField]
    private Image _portraitImage;

    [SerializeField]
    private Image _backgroundImage;

    [SerializeField]
    private Text _nameText;

    [Header("Stats Display Requirements")]

    [SerializeField]
    private Text _coinDisplayText;

    [SerializeField]
    private Text _handCardsDisplayText;

    [SerializeField]
    private Text _buildingsBuiltDisplayText;

    [SerializeField]
    private Image _skillIconImage;

    private CanvasGroup _canvasGroup;

    private GamePlayer _gamePlayerDisplaying = null;
    private FactionType _factionTypeDisplaying;

    private PlayfieldST _playfieldSceneTracker;

    public void DisplayGamePlayer(GamePlayer gameplayer)
    {
        UnDisplayGamePlayer();
        _gamePlayerDisplaying = gameplayer;
        SetFactionSpecifics(_gamePlayerDisplaying.FactionType);

        DisplayActivePlayer(gameplayer.LinkedPlayer);

        gameplayer.LinkedPlayer.RegisteredPlayerConnectedEvent += OnRegisteredPlayerConnectedEvent;
        gameplayer.LinkedPlayer.RegisteredPlayerDisconnectedEvent += OnRegisteredPlayerDisconnectedEvent;

        gameplayer.ReceivedCardEvent += OnReceivedCardEvent;
        gameplayer.PlayCardEvent += OnPlayCardEvent;

        gameplayer.CoinAmountChangedEvent += OnReceivedCoinEvent;

        UpdateStats();
    }

    private void OnPlayCardEvent(GamePlayer gamePlayer, BaseCard card)
    {
        UpdateStats();
    }

    private void OnReceivedCardEvent(GamePlayer gamePlayer, BaseCard card)
    {
        UpdateStats();
    }

    private void UpdateStats()
    {
        if (_gamePlayerDisplaying == null) { return; }

        _handCardsDisplayText.text = _gamePlayerDisplaying.CardsInHand.Length.ToString();
        _coinDisplayText.text = _gamePlayerDisplaying.GoldAmount.ToString();
        _buildingsBuiltDisplayText.text = _playfieldSceneTracker.Playfield.GetCornerByFaction(_gamePlayerDisplaying.FactionType).TotalScoreOfAllBuiltBuildings().ToString();
    }

    public void SetFactionSpecifics(FactionType factionType)
    {
        FactionLibraryItem item = Ramses.Confactory.ConfactoryFinder.Instance.Get<ConPlayerFactions>().FactionsLibrary.GetItemByFactionType(factionType);
        _portraitImage.sprite = item.FactionPortrait;
        _backgroundImage.sprite = item.FactionSectionBackground;
        _nameText.color = item.FactionColor;
        _nameText.text = item.FactionType.ToString();

        _factionTypeDisplaying = factionType;
    }

    public void ToggleActivePlayerDisplay(bool activeState)
    {
        IsActiveDisplay = activeState;
        _canvasGroup.alpha = (IsActiveDisplay) ? 1.0f : _inactiveDisplayAlpha;

        if(_gamePlayerDisplaying == null)
        {
            _handCardsDisplayText.text = "-";
            _coinDisplayText.text = "-";
            _buildingsBuiltDisplayText.text = "-";
            _skillIconImage.gameObject.SetActive(false);
        }
    }

    protected void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _playfieldSceneTracker = SceneTrackersFinder.Instance.GetSceneTracker<PlayfieldST>();
    }

    private void OnRegisteredPlayerConnectedEvent(RegisteredPlayer player)
    {
        DisplayActivePlayer(player);
    }

    private void OnRegisteredPlayerDisconnectedEvent(RegisteredPlayer player)
    {
        DisplayInactivePlayer();
    }

    private void UnDisplayGamePlayer()
    {
        if (_gamePlayerDisplaying == null) { return; }

        DisplayInactivePlayer();

        _gamePlayerDisplaying.LinkedPlayer.RegisteredPlayerConnectedEvent -= OnRegisteredPlayerConnectedEvent;
        _gamePlayerDisplaying.LinkedPlayer.RegisteredPlayerDisconnectedEvent -= OnRegisteredPlayerConnectedEvent;

        _gamePlayerDisplaying.ReceivedCardEvent -= OnReceivedCardEvent;
        _gamePlayerDisplaying.PlayCardEvent -= OnPlayCardEvent;
        _gamePlayerDisplaying.CoinAmountChangedEvent += OnReceivedCoinEvent;

        _gamePlayerDisplaying = null;
    }

    private void OnReceivedCoinEvent(GamePlayer gamePlayer)
    {
        UpdateStats();
    }

    private void DisplayActivePlayer(RegisteredPlayer player)
    {
        ToggleActivePlayerDisplay(true);
        _nameText.text = AirConsole.instance.GetNickname(player.DeviceID);
    }

    private void DisplayInactivePlayer()
    {
        ToggleActivePlayerDisplay(false);
        _nameText.text = _factionTypeDisplaying.ToString();
    }
}
