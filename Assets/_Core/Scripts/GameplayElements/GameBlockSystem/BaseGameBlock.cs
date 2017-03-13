using System;
using UnityEngine;

[Serializable]
public abstract class BaseGameBlock<G ,T, U> : ScriptableGameBlock, IGameBlock<G>
    where G : class, IGame
    where T : struct, IGameBlockInfo<G>
    where U : BaseGameBlockLogic<G, T>
{
    public abstract T BlockInfo { get; }

    IGameBlockInfo<G> IGameBlock<G>.BlockInfo
    {
        get
        {
            return BlockInfo;
        }
    }

    public Type GetGameLogicType()
    {
        return typeof(U);
    }
}

public class ScriptableGameBlock : ScriptableObject
{

}

public interface IGameBlock<T> where T : class, IGame
{
    IGameBlockInfo<T> BlockInfo { get; }
    Type GetGameLogicType();
}

public interface IGameBlockInfo<T> where T : class, IGame
{

}

[Serializable]
public abstract class BaseGameBlockLogic<T, U> : IGameBlockLogic<T> where T : class, IGame where U : struct, IGameBlockInfo<T> 
{
    protected T game { get { return _gameBlockSystem.Game; } }
    protected U gameBlockInfo { get; private set; }
    private BaseGameBlockSystem<T> _gameBlockSystem;
    private bool _initialized = false;
    private bool _active = false;

    public void Initialize(BaseGameBlockSystem<T> gameBlockSystem, IGameBlockInfo<T> gameBlockInfo)
    {
        U info = (U)gameBlockInfo;
        Initialize(gameBlockSystem, info);
    }

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

    public void Destroy()
    {
        if (!_initialized) { return; }
        Deactivate();
        Destroyed();
        _gameBlockSystem.BlockCycleStartedEvent -= OnBlockCycleStartedEvent;
        _gameBlockSystem.BlockCycleEndededEvent -= OnBlockCycleEndededEvent;
    }

    public void Activate()
    {
        if (_active) { return; }
        _active = true;
        Activated();
    }

    public void Deactivate()
    {
        if (!_active) { return; }
        _active = false;
        Deactivated();
    }

    protected void NextBlock()
    {
        _gameBlockSystem.NextBlock();
    }

    protected abstract void Initialized();
    protected abstract void Activated();
    protected abstract void Deactivated();
    protected abstract void CycleStarted();
    protected abstract void CycleEnded();
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

public interface IGameBlockLogic<T> where T : class, IGame
{
    void Initialize(BaseGameBlockSystem<T> gameBlockSystem, IGameBlockInfo<T> gameBlockInfo);
    void Destroy();
    void Activate();
    void Deactivate();
}