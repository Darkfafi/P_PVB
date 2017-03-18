using System;
using UnityEngine;

/// <summary>
/// This class is a block which is used to effect the flow of the game. This is part of the BaseGameBlockSystem. 
/// </summary>
/// <typeparam name="G">The game the GameBlock is linked to</typeparam>
/// <typeparam name="T">The editable information this block contains</typeparam>
/// <typeparam name="U">The functionality of the block. This will use the information provided in T</typeparam>
[Serializable]
public abstract class BaseGameBlock<G ,T, U> : ScriptableGameBlock, IGameBlock<G>
    where G : class, IGame
    where T : struct, IGameBlockInfo<G>
    where U : BaseGameBlockLogic<G, T>
{
    /// <summary>
    /// The info which the BaseGameBlockLogic can use.
    /// </summary>
    public abstract T BlockInfo { get; }

    IGameBlockInfo<G> IGameBlock<G>.BlockInfo
    {
        get
        {
            return BlockInfo;
        }
    }

    /// <summary>
    /// Returns the 'BaseGameBlockLogic' type given in the generic as 'U'
    /// </summary>
    /// <returns></returns>
    public Type GetGameLogicType()
    {
        return typeof(U);
    }
}

public class ScriptableGameBlock : ScriptableObject
{

}

/// <summary>
/// The interface for a GameBlock. This is used in the system for the 'BaseGameBlock' class
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IGameBlock<T> where T : class, IGame
{
    IGameBlockInfo<T> BlockInfo { get; }
    Type GetGameLogicType();
}

/// <summary>
/// The interface for the info which is given to the GameBlocks. This info is used in the GameBlockSystem and is always expected to be of 'struct' type!
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IGameBlockInfo<T> where T : class, IGame
{

}

/// <summary>
/// The base class for the functionality of the GameBlock. This part will be created in the game and used to controll the gameflow.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="U"></typeparam>
[Serializable]
public abstract class BaseGameBlockLogic<T, U> : IGameBlockLogic<T> where T : class, IGame where U : struct, IGameBlockInfo<T> 
{
    /// <summary>
    /// The linked game.
    /// </summary>
    protected T game { get { return _gameBlockSystem.Game; } }
    /// <summary>
    /// The information of the GameBlock. (The part which is editable in the editor)
    /// </summary>
    protected U gameBlockInfo { get; private set; }
    private BaseGameBlockSystem<T> _gameBlockSystem;
    private bool _initialized = false;
    private bool _active = false;

    /// <summary>
    /// Initializes the GameBlock (WARNING: May only be called by the BaseGameBlockSystem!)
    /// </summary>
    public void Initialize(BaseGameBlockSystem<T> gameBlockSystem, IGameBlockInfo<T> gameBlockInfo)
    {
        U info = (U)gameBlockInfo;
        Initialize(gameBlockSystem, info);
    }
    /// <summary>
    /// Initializes the GameBlock (WARNING: May only be called by the BaseGameBlockSystem!)
    /// </summary>
    public void Initialize(BaseGameBlockSystem<T> gameBlockSystem, U gameBlockInfo)
    {
        if (_initialized) { return; }
        _initialized = true;
        this.gameBlockInfo = gameBlockInfo;
        _gameBlockSystem = gameBlockSystem;

        _gameBlockSystem.BlockCycleStartedEvent += OnBlockCycleStartedEvent;
        _gameBlockSystem.BlockCycleEndededEvent += OnBlockCycleEndededEvent;

        Initialized();
    }

    /// <summary>
    /// Tells the GameBlock to be destroyed (WARNING: May only be called by the BaseGameBlockSystem!)
    /// </summary>
    public void Destroy()
    {
        if (!_initialized) { return; }
        Deactivate();
        Destroyed();
        _gameBlockSystem.BlockCycleStartedEvent -= OnBlockCycleStartedEvent;
        _gameBlockSystem.BlockCycleEndededEvent -= OnBlockCycleEndededEvent;
    }
    /// <summary>
    /// Tells the GameBlock to activate (WARNING: May only be called by the BaseGameBlockSystem!)
    /// </summary>
    public void Activate()
    {
        if (_active) { return; }
        _active = true;
        Activated();
    }
    /// <summary>
    /// Tells the GameBlock to DEACTIVATE (WARNING: May only be called by the BaseGameBlockSystem!)
    /// </summary>
    public void Deactivate()
    {
        if (!_active) { return; }
        _active = false;
        Deactivated();
    }
    /// <summary>
    /// This method is used by the GameBlockLogic to tell the system to go to the next block and deactivate this one.
    /// </summary>
    protected void NextBlock()
    {
        _gameBlockSystem.NextBlock();
    }

    /// <summary>
    /// Called on Initialization (Once per run)
    /// </summary>
    protected abstract void Initialized();
    /// <summary>
    /// Called on Activation of the block (Everytime the cycle lands on this GameBlock)
    /// </summary>
    protected abstract void Activated();
    /// <summary>
    /// Called on Deactivation of the block (Everytime the cycle continues to the next GameBlock or the cycle is ended)
    /// </summary>
    protected abstract void Deactivated();
    /// <summary>
    /// Called when the Cycle is started of the BaseGameBlockSystem (Called everytime the system is started)
    /// </summary>
    protected abstract void CycleStarted();
    /// <summary>
    /// Called when the Cycle is ende of the BaseGameBlockSystem (Called everytime the system is ended)
    /// </summary>
    protected abstract void CycleEnded();
    /// <summary>
    /// Called when the system is destroyed (Once per run)
    /// </summary>
    protected abstract void Destroyed();

    private void OnBlockCycleStartedEvent()
    {
        CycleStarted();
    }

    private void OnBlockCycleEndededEvent()
    {
        CycleEnded();
    }
}

/// <summary>
/// The interface for a GameBlockLogic. Used in the system for the 'BaseGameBlockLogic' class
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IGameBlockLogic<T> where T : class, IGame
{
    void Initialize(BaseGameBlockSystem<T> gameBlockSystem, IGameBlockInfo<T> gameBlockInfo);
    void Destroy();
    void Activate();
    void Deactivate();
}