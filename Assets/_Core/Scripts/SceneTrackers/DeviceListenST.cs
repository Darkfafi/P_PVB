using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ramses.SceneTrackers;
using System;

public class DeviceListenST : MonoBehaviour, ISceneTracker
{
    public void SystemAwakeCall()
    {
        // Gets the ConPlayers at the start of the Scene to track the players connecting.
        Ramses.Confactory.ConfactoryFinder.Instance.Get<ConPlayers>(); 
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
