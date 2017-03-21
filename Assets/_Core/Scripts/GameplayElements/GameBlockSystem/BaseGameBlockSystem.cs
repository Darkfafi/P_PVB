using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// This system determains the flow of the game by BaseGameBlocks. These contain logics and will be played in order when this system is activated and will loop until it is stopped.
/// </summary>
/// <typeparam name="T">The game class which holds all the game data and has full controll over this class.</typeparam>
[Serializable]
public abstract class BaseGameBlockSystem<T> where T : class, IGame
{
    public delegate void BaseGameBlockHandler(IGameBlockLogic<T> logic);
    
    /// <summary>
    /// This event will be triggered when a GameBlock is switched. This will happen when a gameblock is ended by the 'NextBlock' method
    /// </summary>
    public event BaseGameBlockHandler BlockSwitchedEvent;
    /// <summary>
    /// This event will be triggered when the Block Cycle is started with the 'StartBlockCycle' method
    /// </summary>
    public event VoidHandler BlockCycleStartedEvent;
    /// <summary>
    /// This event will be triggered when the Block Cycle is ended with the 'EndBlockCycle' method
    /// </summary>
    public event VoidHandler BlockCycleEndededEvent;

    /// <summary>
    /// The game linked to the BaseGameBlockSystem
    /// </summary>
    public T Game { get; private set; }

    private IGameBlockLogic<T>[] _gameBlocks;
    private int _currentBlockIndex = 0;
    private bool _isDestroyed = false; 

    /// <summary>
    /// Here the GameBlocks are given to the system to work with. The order it is given is the order it will activate them.
    /// </summary>
    /// <param name="gameInstance">The Game Linked to this system</param>
    /// <param name="buildingBlocks">The building blocks in correct order</param>
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
    /// <summary>
    /// Here the GameBlocks are given to the system to work with. The order it is given is the order it will activate them.
    /// </summary>
    /// <param name="gameInstance">The Game Linked to this system</param>
    /// <param name="buildingBlocks">The building blocks in correct order</param>
    public BaseGameBlockSystem(T gameInstance, IGameBlock<T>[] buildingBlocks)
    {
        Initialization(gameInstance, buildingBlocks);
    }

    ~BaseGameBlockSystem()
    {
        Destroy();
    }

    /// <summary>
    /// Starts the BlockCycle and will loop it until it is ended with the 'EndBlockCycle' method
    /// </summary>
    public void StartBlockCycle()
    {
        if (BlockCycleStartedEvent != null)
            BlockCycleStartedEvent();

        _currentBlockIndex = -1;
        NextBlock();
    }

    /// <summary>
    /// This will deactivate the current GameBlock and activate the next. If it is the last GameBlock, it will go to the first.
    /// </summary>
    public void NextBlock()
    {
        EndCurrentBlock();

        _currentBlockIndex = _gameBlocks.GetLoopIndex(_currentBlockIndex + 1);

        _gameBlocks[_currentBlockIndex].Activate();

        if (BlockSwitchedEvent != null)
            BlockSwitchedEvent(_gameBlocks[_currentBlockIndex]);
    }
    /// <summary>
    /// Ends the BlockCycle and will deactivate the current active block
    /// </summary>
    public void EndBlockCycle()
    {
        EndCurrentBlock();
        _currentBlockIndex = -1;
        if (BlockCycleEndededEvent != null)
            BlockCycleEndededEvent();
    }

    /// <summary>
    /// This method must be called for it to be fully destroyed out of memory. This will also call the 'Destroy' method in every GameBlock in the system.
    /// </summary>
    public void Destroy()
    {
        if (_isDestroyed) { return; }
        for (int i = 0; i < _gameBlocks.Length; i++)
        {
            _gameBlocks[i].Destroy();
        }
        _isDestroyed = true;
    }

    /// <summary>
    /// Handles initialization because it has multiple constructors.
    /// </summary>
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

    /// <summary>
    /// Ends the currently active GameBlock. (If there is any)
    /// </summary>
    private void EndCurrentBlock()
    {
        if (_currentBlockIndex != -1)
        {
            _gameBlocks[_currentBlockIndex].Deactivate();
        }
    }
}
