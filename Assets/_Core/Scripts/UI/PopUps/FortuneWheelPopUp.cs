using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class FortuneWheelPopUp : BasePopUp
{
    private const string SPIN_START_TRIGGER = "StartSpin";
    private const string SPIN_RESULT_INT = "Result";

    [SerializeField]
    private float _waitAfterEndInSeconds = 1.2f;

    [SerializeField]
    private Image[] _corners;
    private Animator _animator;

    private List<int> _winnerIndexes = new List<int>();
    private float _timeWaited = 0;

    protected override void Open()
    {

    }

    protected void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Spin(FactionType[] factionsToDisplay, int indexToWin)
    {
        _timeWaited = 0;
        SetCorners(factionsToDisplay, indexToWin);
        _animator.SetTrigger(SPIN_START_TRIGGER);

    }

    protected void Update()
    {
        if(_animator.GetBool("Ended"))
        {
            _timeWaited += Time.deltaTime;
            if (_timeWaited >= _waitAfterEndInSeconds)
                ClosePopUp();
        }
        _timeWaited = 0;
    }

    private void SetCorners(FactionType[] factionsToDisplay, int indexToWin)
    {
        bool useAllCorners = factionsToDisplay.Length != 3;
        FactionType factionToShow = FactionType.None;
        _winnerIndexes.Clear();
        for (int i = 0; i < _corners.Length; i++)
        {
            _corners[i].gameObject.SetActive(true);
            factionToShow = factionsToDisplay.GetLoop(i);
            _corners[i].sprite = Ramses.Confactory.ConfactoryFinder.Instance.Get<ConPlayerFactions>().FactionsLibrary.GetItemByFactionType(factionToShow).FactionFortuneWheelCorner;
            if (!useAllCorners && i == _corners.Length - 1)
            {
                _corners[i].gameObject.SetActive(false);
            }
            else
            {
                if (factionToShow == factionsToDisplay[indexToWin])
                    _winnerIndexes.Add(i);
            }
        }

        _animator.SetInteger(SPIN_RESULT_INT, _winnerIndexes[UnityEngine.Random.Range(0, _winnerIndexes.Count)]);
    }

   
    private void ClosePopUp()
    {
        Close();
    }
}
