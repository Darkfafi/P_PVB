using NDream.AirConsole;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class JoinTab : MonoBehaviour
{
    public RegisteredPlayer DisplayingPlayer { get; private set; }
    public bool IsReady { get; private set; }

    [Header("Options")]
    [SerializeField]
    private Color _userInTabTextColor = Color.blue;
    [SerializeField]
    private Color _noUserInTabTextColor = Color.white;
    [SerializeField]
    private Sprite _guestProfileImage;

    [Header("Requirements")]
    [SerializeField]
    private Image _readyImage;

    [SerializeField]
    private Image _playerImage;

    [SerializeField]
    private Text _usernameText;

    protected void Awake()
    {
        _readyImage.gameObject.SetActive(false);
        DisplayPlayer(null);
    }

    public void DisplayPlayer(RegisteredPlayer player)
    {
        ToggleReady(false);

        if (player != null)
        {
            // Player to show
            _usernameText.text = AirConsole.instance.GetNickname(player.DeviceID);
            _usernameText.color = _userInTabTextColor;
            _playerImage.gameObject.SetActive(true);
            _playerImage.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            this.transform.DOScale(0.88f, 0.6f);
            _playerImage.sprite = _guestProfileImage;
            StartCoroutine(SetSprite(player.DeviceID));
        }
        else
        {
            // No player to show
            _usernameText.text = "Open";
            _usernameText.color = _noUserInTabTextColor;
            _playerImage.gameObject.SetActive(false);
        }

        DisplayingPlayer = player;

    }

    public void ToggleReady(bool ready)
    {
        if (IsReady == ready) { return; }
        _readyImage.transform.DOKill();
        IsReady = ready;
        if (ready)
        {
            if(!_readyImage.gameObject.activeSelf)
                _readyImage.transform.localScale = Vector3.zero;

            _readyImage.gameObject.SetActive(true);
            _readyImage.transform.DOScale(1, 0.5f).SetEase(Ease.OutElastic, 1, 0.4f);
        }else
        {
            _readyImage.transform.DOScale(0.0f, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
            {
                _readyImage.gameObject.SetActive(false);
            });
        }
    }

    private IEnumerator SetSprite(int playerId)
    {
        WWW profilePictureWWW = new WWW(AirConsole.instance.GetProfilePicture(playerId));
        yield return profilePictureWWW;
        _playerImage.sprite = Sprite.Create(profilePictureWWW.texture, new Rect(0,0, profilePictureWWW.texture.width, profilePictureWWW.texture.height), new Vector2(0, 0));
        _playerImage.transform.DOScale(1, 0.2f).SetEase(Ease.OutCubic);
        this.transform.DOKill();
        this.transform.DOScale(1f, 0.8f).SetEase(Ease.OutElastic);
    }
}
