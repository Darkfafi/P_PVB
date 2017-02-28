using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClosablePopUp : BasePopUp {

    protected Button closeButton { get { return _closeButton; } }

    [Header("Closable Pop-up settings")]
    [SerializeField]
    private Button _closeButton;

	protected virtual void Awake()
    {
        _closeButton.onClick.AddListener(OnCloseClicked);
    }

    protected virtual void OnCloseClicked()
    {
        Close();
    }

    protected override void OnDestroy()
    {
        _closeButton.onClick.RemoveListener(OnCloseClicked);
        base.OnDestroy();
    }

    protected override void Open()
    {

    }
}
