using UnityEngine;
using Ramses.SceneTrackers;
using System;

public class BuildingsGameST : MonoBehaviour, ISceneTracker {

    public BuildingsGame BuildingsGame
    {
        get { return _buildingsGame; }
    }

    [SerializeField]
    private BuildingsGame _buildingsGame;

    public void SystemAwakeCall()
    {

    }

    public void SystemStartCall()
    {

    }

    public void SystemDestroyCall()
    {

    }

    public void SystemRequested()
    {

    }
}
