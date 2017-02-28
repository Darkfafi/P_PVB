using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class PopUpSystem : MonoBehaviour
{
    public const string RESOURCE_PATH_POP_UP_SYSTEM = "PopUpSystemResources";

    public delegate void PopUpHandler(BasePopUp popUp);
    public event PopUpHandler PopUpCreatedEvent;

    public static PopUpSystem Instance
    {
        get 
        {
            if(_instance == null)
            {
                _instance = Instantiate<PopUpSystem>(Resources.Load<PopUpSystem>(RESOURCE_PATH_POP_UP_SYSTEM + "/PopUpSystem"));
                _instance.name = "{" + _instance.name + "}";
                DontDestroyOnLoad(_instance);
            }
            return _instance;
        }
    }

    private static PopUpSystem _instance = null;

    [SerializeField] private Canvas popUpCanvas;
    private PopUpTracker popUpTracker = new PopUpTracker();

    public T CreatePopUp<T>(string popUpResourceLocation, bool coverPopUp = true, int layer = 0, bool destroyOnNewScene = true) where T : BasePopUp
    {
        return CreatePopUp<T>(Resources.Load<T>(popUpResourceLocation), coverPopUp, layer, destroyOnNewScene);
    }

    public T CreatePopUp<T>(T popUpPrefab, bool coverPopUp = true, int layer = 0, bool destroyOnNewScene = true) where T : BasePopUp
    {
        T instancePopUp = GameObject.Instantiate<T>(popUpPrefab);
        Image background = null;

        instancePopUp.name = "[" + instancePopUp.name + "]";

        if (coverPopUp)
        {
            background = Instantiate<Image>(Resources.Load<Image>(RESOURCE_PATH_POP_UP_SYSTEM + "/PopUpBackground"));
            background.transform.SetParent(popUpCanvas.transform, false);
            instancePopUp.transform.SetParent(background.transform, false);
        }
        else
        {
            instancePopUp.transform.SetParent(popUpCanvas.transform, false);
        }

        instancePopUp.Initialize(background, popUpTracker, popUpCanvas, layer, destroyOnNewScene);
        popUpTracker.AddPopUp(instancePopUp);
        instancePopUp.MonoOpenAfterInit();

        if(PopUpCreatedEvent != null)
        {
            PopUpCreatedEvent(instancePopUp);
        }

        return instancePopUp;
    }

    public bool IsPopUpActive<T>() where T : BasePopUp
    {
        return GetPopUp<T>() != null;
    }

    public void ClosePopUp<T>() where T : BasePopUp
    {
        T popUp = GetPopUp<T>();
        if(popUp != null)
        {
            popUp.Close();
        }
    }

    public T GetPopUp<T>() where T : BasePopUp
    {
        BasePopUp[] activePopUps = popUpTracker.AllActivePopUps;
        for (int i = 0; i < activePopUps.Length; i++)
        {
            if (activePopUps[i].GetType() == typeof(T))
            {
                return (T)activePopUps[i];
            }
        }

        return null;
    }
}

public class PopUpTracker
{
    public int PopUpsActiveAmount { get { return allActivePopUps.Count; } }
    public BasePopUp[] AllActivePopUps { get { return allActivePopUps.ToArray(); } }
    private List<BasePopUp> allActivePopUps = new List<BasePopUp>();

    public bool AddPopUp(BasePopUp popUp)
    {
        if (!HasPopUp(popUp))
        {
            popUp.PopUpBeingDestroyedEvent += OnPopUpBeingDestroyedEvent;
            allActivePopUps.Add(popUp);
            UpdateList();
            return true;
        }

        return false;
    }

    public BasePopUp GetTopPopUp()
    {
        if (allActivePopUps.Count == 0) { return null; }
        return allActivePopUps[allActivePopUps.Count - 1];
    }

    public bool HasPopUp(BasePopUp popUp)
    {
        return allActivePopUps.Contains(popUp);
    }

    public int GetIndexOf(BasePopUp popUp)
    {
        if(HasPopUp(popUp))
        {
            return allActivePopUps.IndexOf(popUp);
        }
        return -1;
    }

    private void OnPopUpBeingDestroyedEvent(BasePopUp popUpEffected)
    {
        if (allActivePopUps.Contains(popUpEffected))
        {
            popUpEffected.PopUpBeingDestroyedEvent -= OnPopUpBeingDestroyedEvent;
            allActivePopUps.Remove(popUpEffected);
            UpdateList();
        }
    }

    private void UpdateList()
    {
        allActivePopUps.Sort(LayerSort);
        for (int i = 0; i < allActivePopUps.Count; i++)
        {
            allActivePopUps[i].GetIndexableTransform().SetSiblingIndex(i);
            allActivePopUps[i].CheckIfTopPopUp();
        }
    }

    private int LayerSort(BasePopUp x, BasePopUp y)
    {
        if (x.SortingLayer < y.SortingLayer)
            return -1;
        if (x.GetInstanceID() < y.GetInstanceID())
            return 1;
        return -1;
    }
}
