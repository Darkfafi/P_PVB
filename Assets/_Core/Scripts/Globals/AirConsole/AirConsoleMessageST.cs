using System.Collections.Generic;
using UnityEngine;
using Ramses.SceneTrackers;
using System;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

/// <summary>
/// This SceneTracker contains the Get<> method which you can use to get BaseACMessageTranslators. 
/// These will be created in this scene if not already active.
/// The translators handle the communication between Controller and Game for certain subject.
/// </summary>
public class AirConsoleMessageST : MonoBehaviour, ISceneTracker
{
    public const string INFO_PARAMETER = "info";

    private Dictionary<Type, BaseACMessageTranslator> _translators = new Dictionary<Type, BaseACMessageTranslator>();


    /// <summary>
    /// This method returns a class which inherrits from the 'BaseACMessageTranslator' class.
    /// If not already active the translator will be created.
    /// </summary>
    /// <typeparam name="T">Translator type of choice</typeparam>
    /// <returns></returns>
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

/// <summary>
/// A translator is responceable for the communication links between the game and the controller.
/// </summary>
public abstract class BaseACMessageTranslator
{
    public void DirectMessage(int from, JToken data)
    {
        MessageReceived(from, data);
    }

    protected string CreateParsableString(params int[] separateStringsToBind)
    {
        string[] s = new string[separateStringsToBind.Length];
        for (int i = 0; i < s.Length; i++)
        {
            s[i] = separateStringsToBind[i].ToString();
        }
        return CreateParsableString(s);
    }

    protected string CreateParsableString(params string[] separateStringsToBind)
    {
        string binds = "";

        for (int i = 0; i < separateStringsToBind.Length; i++)
        {
            binds += separateStringsToBind[i];
            if (i < separateStringsToBind.Length - 1)
            {
                binds += ",";
            }
        }
        return binds;
    }

    protected abstract void MessageReceived(int from, JToken data);
}
