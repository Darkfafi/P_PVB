using NDream.AirConsole;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinTab : MonoBehaviour
{
    public RegisteredPlayer DisplayingPlayer { get; private set; }
    public bool IsReady { get { return _readyImage.gameObject.activeSelf; } }

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
        _readyImage.gameObject.SetActive(ready);
    }

    private IEnumerator SetSprite(int playerId)
    {
        WWW profilePictureWWW = new WWW(AirConsole.instance.GetProfilePicture(playerId));
        yield return profilePictureWWW;
        _playerImage.sprite = Sprite.Create(profilePictureWWW.texture, new Rect(0,0, profilePictureWWW.texture.width, profilePictureWWW.texture.height), new Vector2(0, 0));
    }
}
