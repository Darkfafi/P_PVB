﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ramses.SceneTrackers;
using System;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class AirConsoleMessageST : MonoBehaviour, ISceneTracker
{
    public const string INFO_PARAMETER = "info";

    private Dictionary<Type, BaseACMessageTranslator> _translators = new Dictionary<Type, BaseACMessageTranslator>();

    public T Get<T>() where T : BaseACMessageTranslator
    {
        Type t = typeof(T);

        if (_translators.ContainsKey(t))
        {
            return (T)_translators[t];
        }

        T newTranslator = Activator.CreateInstance<T>();
        _translators.Add(t, newTranslator);

        return newTranslator;
    }

    public void SystemAwakeCall()
    {
        AirConsole.instance.onMessage += OnMessageEvent;
    }

    public void SystemStartCall()
    {

    }

    public void SystemRequested()
    {

    }

    public void SystemDestroyCall()
    {
        AirConsole.instance.onMessage -= OnMessageEvent;
    }

    private void OnMessageEvent(int from, JToken data)
    {
        foreach(var translator in _translators)
        {
            translator.Value.DirectMessage(from, data);
        }
    }
}

public abstract class BaseACMessageTranslator
{
    public void DirectMessage(int from, JToken data)
    {
        MessageReceived(from, data);
    }

    protected abstract void MessageReceived(int from, JToken data);
}