using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

[Serializable]
public abstract class BaseGameBlockSystem<T> where T : class, IGame
{
    public delegate void BaseGameBlockHandler(IGameBlockLogic<T> logic);

    public event BaseGameBlockHandler BlockSwitchedEvent;
    public event VoidHandler BlockCycleStartedEvent;
    public event VoidHandler BlockCycleEndededEvent;

    public T Game { get; private set; }

    private IGameBlockLogic<T>[] _gameBlocks;
    private int _currentBlockIndex = 0;
    private bool _isDestroyed = false; 

    public BaseGameBlockSystem(T gameInstance, ScriptableGameBlock[] buildingBlocks)
    {
        List<IGameBlock<T>> gameBlockItems = new List<IGameBlock<T>>();
        for(int i = 0; i < buildingBlocks.Length; i++)
        {
            IGameBlock<T> item = buildingBlocks[i] as IGameBlock<T>;
            if (item != null)
                gameBlockItems.Add(item);
            else
                Debug.LogError("Items must be of type: '" + typeof(T) +"'!");
        }

        Initialization(gameInstance, gameBlockItems.ToArray());
    }

    public BaseGameBlockSystem(T gameInstance, IGameBlock<T>[] buildingBlocks)
    {
        Initialization(gameInstance, buildingBlocks);
    }

    ~BaseGameBlockSystem()
    {
        Destroy();
    }

    public void StartBlockCycle()
    {
        if (BlockCycleStartedEvent != null)
            BlockCycleStartedEvent();

        _currentBlockIndex = -1;
        NextBlock();
    }

    public void NextBlock()
    {
        EndCurrentBlock();

        _currentBlockIndex = _gameBlocks.GetLoopIndex(_currentBlockIndex + 1);

        _gameBlocks[_currentBlockIndex].Activate();

        if (BlockSwitchedEvent != null)
            BlockSwitchedEvent(_gameBlocks[_currentBlockIndex]);
    }

    public void EndBlockCycle()
    {
        EndCurrentBlock();

        if (BlockCycleEndededEvent != null)
            BlockCycleEndededEvent();
    }


    public void Destroy()
    {
        if (_isDestroyed) { return; }
        for (int i = 0; i < _gameBlocks.Length; i++)
        {
            _gameBlocks[i].Destroy();
        }
        _isDestroyed = true;
    }

    private void Initialization(T gameInstance, IGameBlock<T>[] buildingBlocks)
    {
        _gameBlocks = new IGameBlockLogic<T>[buildingBlocks.Length];
        Game = gameInstance;
        for (int i = 0; i < buildingBlocks.Length; i++)
        {
            _gameBlocks[i] = (IGameBlockLogic<T>)Activator.CreateInstance(buildingBlocks[i].GetGameLogicType());
            _gameBlocks[i].Initialize(this, buildingBlocks[i].BlockInfo);
        }
    }

    private void EndCurrentBlock()
    {
        if (_currentBlockIndex != -1)
        {
            _gameBlocks[_currentBlockIndex].Deactivate();
        }
    }
}
