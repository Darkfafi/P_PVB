using UnityEngine;
using Ramses.SceneTrackers;
using System;

public class PlayfieldST : MonoBehaviour, ISceneTracker
{
    public Playfield Playfield { get { return _playfield; } }

    [SerializeField]
    private Playfield _playfield;

    public void SystemAwakeCall()
    {

    }

    public void SystemDestroyCall()
    {

    }

    public void SystemRequested()
    {

    }

    public void SystemStartCall()
    {

    }
}
