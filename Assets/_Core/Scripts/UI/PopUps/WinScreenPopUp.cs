using System;
using UnityEngine.UI;
using UnityEngine;
using NDream.AirConsole;

public class WinScreenPopUp : BasePopUp
{
    [SerializeField]
    private Text _usernameText;

    [SerializeField]
    private Text _scoreValueText;

    [SerializeField]
    private Text _buildingsValueText;

    [SerializeField]
    private Text _cardsValueText;

    [SerializeField]
    private Text _coinsValueText;

    [SerializeField]
    private Image _factionCharacterImage;

    protected override void Open()
    {
        Invoke("GoToLobby", 5f);
    }

    public void SetWinner(GamePlayer winner, PlayerCorner corner)
    {
        _usernameText.text = AirConsole.instance.GetNickname(winner.LinkedPlayer.DeviceID);
        _scoreValueText.text = corner.TotalScoreOfAllBuiltBuildings().ToString();
        _buildingsValueText.text = corner.GetAllBuildFieldsInUse().Length.ToString();
        _cardsValueText.text = winner.CardsInHand.Length.ToString();
        _coinsValueText.text = winner.GoldAmount.ToString();

        FactionsLibrary lib = Ramses.Confactory.ConfactoryFinder.Instance.Get<ConPlayerFactions>().FactionsLibrary;

        _scoreValueText.color = _usernameText.color = _buildingsValueText.color = _cardsValueText.color = _coinsValueText.color = lib.GetItemByFactionType(winner.FactionType).FactionColor;

        _factionCharacterImage.sprite = lib.GetItemByFactionType(winner.FactionType).FactionCharacterSprite;
    }

    private void GoToLobby()
    {
        Ramses.Confactory.ConfactoryFinder.Instance.Get<ConSceneSwitcher>().SwitchScreen(SceneNames.LOBBY_SCENE);
    }
}
