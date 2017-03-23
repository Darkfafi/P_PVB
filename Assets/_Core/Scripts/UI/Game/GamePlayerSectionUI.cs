using NDream.AirConsole;
using UnityEngine;
using UnityEngine.UI;
using Ramses.SceneTrackers;
using System;

/// <summary>
/// This component is the main component for the GamePlayer corner UIs.
/// When linked to a player, it listens to changes in the players stats and displays.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class GamePlayerSectionUI : MonoBehaviour
{
    /// <summary>
    /// If the player is not connected or has never been registered in the first place. This will give false, else it will give true;
    /// </summary>
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

    /// <summary>
    /// This links a player to the UI corner. This will listen to the plaayer his or her actions and set the UI coresponding to them.
    /// PS: this also unlistens from the player if it is already displaying a GamePlayer.
    /// </summary>
    /// <param name="gameplayer">Player to link</param>
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

        gameplayer.SkillPouch.SkillSetEvent += OnSkillSetEvent;

        gameplayer.CoinAmountChangedEvent += OnReceivedCoinEvent;

        UpdateStats();
    }

    /// <summary>
    /// Displays the faction specific parts for the corner, this includes its color / font color and its portrait.
    /// </summary>
    /// <param name="factionType">FactionType to chance appearance to</param>
    public void SetFactionSpecifics(FactionType factionType)
    {
        FactionLibraryItem item = Ramses.Confactory.ConfactoryFinder.Instance.Get<ConPlayerFactions>().FactionsLibrary.GetItemByFactionType(factionType);
        _portraitImage.sprite = item.FactionPortrait;
        _backgroundImage.sprite = item.FactionSectionBackground;
        _nameText.color = item.FactionColor;
        _nameText.text = item.FactionType.ToString();

        _factionTypeDisplaying = factionType;
    }

    /// <summary>
    /// This will display the player as active or inactive player. 
    /// If there is no player linked then the section will be put into complete inactive mode.
    /// </summary>
    /// <param name="activeState"></param>
    public void ToggleActivePlayerDisplay(bool activeState)
    {
        gameObject.SetActive(true);
        IsActiveDisplay = activeState;
        _canvasGroup.alpha = (IsActiveDisplay) ? 1.0f : _inactiveDisplayAlpha;

        if(_gamePlayerDisplaying == null)
        {
            _handCardsDisplayText.text = "-";
            _coinDisplayText.text = "-";
            _buildingsBuiltDisplayText.text = "-";
            _skillIconImage.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    protected void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _playfieldSceneTracker = SceneTrackersFinder.Instance.GetSceneTracker<PlayfieldST>();
    }

    protected void OnDestroy()
    {
        UnDisplayGamePlayer();
    }


    private void OnSkillSetEvent(GamePlayer gamePlayer, Skill skill)
    {
        SkillLibraryItem item = Ramses.Confactory.ConfactoryFinder.Instance.Get<ConSkills>().SkillLibrary.GetSkillItem(skill);
        if (item != null)
        {
            _skillIconImage.sprite = item.SkillIcon;
            _skillIconImage.gameObject.SetActive(true);
        }
        else
        {
            _skillIconImage.gameObject.SetActive(false);
        }
    }

    private void OnPlayCardEvent(GamePlayer gamePlayer, BaseCard card)
    {
        UpdateStats();
    }

    private void OnReceivedCardEvent(GamePlayer gamePlayer, BaseCard card)
    {
        UpdateStats();
    }

    /// <summary>
    /// This will update all the stat displays. This will be triggered when a stat value changes in the linked player.
    /// </summary>
    private void UpdateStats()
    {
        if (_gamePlayerDisplaying == null) { return; }

        _handCardsDisplayText.text = _gamePlayerDisplaying.CardsInHand.Length.ToString();
        _coinDisplayText.text = _gamePlayerDisplaying.GoldAmount.ToString();
        _buildingsBuiltDisplayText.text = _playfieldSceneTracker.Playfield.GetCornerByFaction(_gamePlayerDisplaying.FactionType).TotalScoreOfAllBuiltBuildings().ToString();
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

        _gamePlayerDisplaying.SkillPouch.SkillSetEvent -= OnSkillSetEvent;

        _gamePlayerDisplaying = null;
    }

    private void OnReceivedCoinEvent(GamePlayer gamePlayer)
    {
        UpdateStats();
    }

    /// <summary>
    /// Displays the active player locally.
    /// </summary>
    /// <param name="player"></param>
    private void DisplayActivePlayer(RegisteredPlayer player)
    {
        ToggleActivePlayerDisplay(true);
        _nameText.text = AirConsole.instance.GetNickname(player.DeviceID);
    }

    /// <summary>
    /// Displays the inactive player locally.
    /// </summary>
    private void DisplayInactivePlayer()
    {
        ToggleActivePlayerDisplay(false);
        _nameText.text = _factionTypeDisplaying.ToString();
    }
}
