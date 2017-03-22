using UnityEngine;
using UnityEngine.UI;
using Ramses.Confactory;
using System;
using DG.Tweening;
using NDream.AirConsole;

public class FactionSelect : MonoBehaviour
{
    public FactionType FactionType { get { return _factionType; } }

    [Header("Options")]
    [SerializeField]
    private Color _activeColor;

    [Header("Requirements")]
    [SerializeField]
    private Text _nameText;

    [SerializeField]
    private Image _characterImage;

    [SerializeField]
    private Image _selectionIcon;

    [SerializeField]
    private FactionType _factionType;

    private ConPlayerFactions _conPlayerFactions;

    protected void Awake()
    {
        _conPlayerFactions = ConfactoryFinder.Instance.Get<ConPlayerFactions>();

        _conPlayerFactions.PlayerFactionAssignedEvent += OnPlayerFactionAssignedEvent;
        _conPlayerFactions.PlayerFactionUnassignedEvent += OnPlayerFactionUnassignedEvent;

        AwakeCheckFactionItem();
    }

    protected void OnDestroy()
    {
        _conPlayerFactions.PlayerFactionAssignedEvent -= OnPlayerFactionAssignedEvent;
        _conPlayerFactions.PlayerFactionUnassignedEvent -= OnPlayerFactionUnassignedEvent;
    }

    private void OnPlayerFactionAssignedEvent(PlayerFactionLinkItem linkItem)
    {
        if (linkItem.FactionType != FactionType) { return; }
        SetActive(linkItem.Player);
    }

    private void OnPlayerFactionUnassignedEvent(PlayerFactionLinkItem linkItem)
    {
        if (linkItem.FactionType != FactionType) { return; }
        SetActive(null);
    }

    private void SetActive(RegisteredPlayer player)
    {
        _selectionIcon.transform.DOComplete();

        if (player == null)
        {
            // Inactive
            _selectionIcon.transform.DOScale(0, 0.5f).SetEase(Ease.InBack);

            _characterImage.color = Color.white;
            _nameText.color = _conPlayerFactions.FactionsLibrary.GetItemByFactionType(FactionType).FactionColor;
            _nameText.text = Enum.GetName(typeof(FactionType),FactionType);
        }
        else
        {
            // Active
            _selectionIcon.transform.localScale = Vector3.zero;
            _selectionIcon.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack);

            _characterImage.color = _activeColor;
            _nameText.color = _conPlayerFactions.FactionsLibrary.GetItemByFactionType(FactionType).FactionColor + _activeColor;
            _nameText.text = AirConsole.instance.GetNickname(player.DeviceID);
        }
    }

    private void AwakeCheckFactionItem()
    {
        _selectionIcon.transform.localScale = Vector3.zero;
        PlayerFactionLinkItem item = _conPlayerFactions.GetLinkItemForFaction(FactionType);
        if(item.Player != null)
        {
            OnPlayerFactionAssignedEvent(item);
        }
        else
        {
            OnPlayerFactionUnassignedEvent(item);
        }
    }
}
