using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public abstract class BasePopUp : MonoBehaviour
{
    public delegate void PopUpHandler(BasePopUp popUpEffected);
    public event PopUpHandler PopUpBeingDestroyedEvent;

    public bool IsCoverPopUp { get { return coverBackground != null; } }
    public bool IsTopPopUp { get { return popUpTracker.GetTopPopUp() == this; } }
    public bool IsInteractable 
    {
        get {
            if (IsTopPopUp) { return true; }
            for (int i = popUpTracker.GetIndexOf(this) + 1; i < popUpTracker.PopUpsActiveAmount; i++)
            {
                if (popUpTracker.AllActivePopUps[i].IsCoverPopUp)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public int SortingLayer { get; private set; }

    protected Canvas popUpSystemCanvas { get; private set; }

    protected Image coverBackground { get; private set; }
    protected Color originalCoverBgColor { get; private set; }

    private PopUpTracker popUpTracker;

    // Data safety variables
    private bool isBeingDestroyed = false;
    private bool hasBeenInitialized = false;
    private bool hasBeenOpenedAfterInit = false;

    // Creation set tracking variables
    private string setName;
    private string nameExtenstisetNameAddon = "";

    protected abstract void Open(); // Can be used for opening effect

    public void Initialize(Image coverBackground, PopUpTracker popUpTracker, Canvas popUpSystemCanvas, int addedToLayer, bool destroyOnNewSceneLoad)
    {
        if (!hasBeenInitialized)
        {
            SortingLayer = addedToLayer;
            setName = gameObject.name;
            nameExtenstisetNameAddon = "";
            hasBeenInitialized = true;
            if (coverBackground != null)
            {
                this.coverBackground = coverBackground;
                originalCoverBgColor = coverBackground.color;
            }
            this.popUpTracker = popUpTracker;
            this.popUpSystemCanvas = popUpSystemCanvas;
            UpdateName();

            if (destroyOnNewSceneLoad)
                SceneManager.sceneUnloaded += OnNewSceneLoaded;
        }
    }

    private void OnNewSceneLoaded(Scene newScene)
    {
        DestroyPopUpObject();
    }

    public void MonoOpenAfterInit()
    {
        if(!hasBeenOpenedAfterInit)
        {
            Open();
            hasBeenOpenedAfterInit = true;
        }
        else
        {
            Debug.LogWarning("The pop-up has already been opened after the init. Thus this method can not be called.");
        }
    }

    public bool CheckIfTopPopUp()
    {
        bool isTop = IsTopPopUp;

        Color c = originalCoverBgColor;

        if (coverBackground != null)
            c = coverBackground.color;

        if (IsHighestCoverPopUp())
            c.a = originalCoverBgColor.a;   
        else
            c.a = 0;

        if (coverBackground != null)
            coverBackground.color = c;

        if(!IsInteractable)
        {
            nameExtenstisetNameAddon = "(X)";
        }
        else
        {
            nameExtenstisetNameAddon = "";
        }

        UpdateName();
        OnOrderAndCoverCheck();
        return isTop;
    }

    public virtual void Close() // Can be used for close effect
    {
        DestroyPopUpObject();
    }

    public bool IsHighestCoverPopUp()
    {
        if (!IsCoverPopUp) { return false; }
        int index = popUpTracker.GetIndexOf(this);
        if (index == popUpTracker.PopUpsActiveAmount - 1)
        {
            return IsCoverPopUp;
        }

        for (int i = popUpTracker.GetIndexOf(this) + 1; i < popUpTracker.PopUpsActiveAmount; i++)
        {
            if (popUpTracker.AllActivePopUps[i].IsCoverPopUp)
            {
                return false;
            }
        }
        return true;
    }

    public bool IsLowestCoverPopUp()
    {
        if (!IsCoverPopUp) { return false; }
        int index = popUpTracker.GetIndexOf(this);
        if (index == 0)
        {
            return IsCoverPopUp;
        }

        for (int i = popUpTracker.GetIndexOf(this) - 1; i >= 0; i--)
        {
            if (popUpTracker.AllActivePopUps[i].IsCoverPopUp)
            {
                return false;
            }
        }

        return true;
    }

    public Transform GetIndexableTransform()
    {
        if (coverBackground != null)
            return coverBackground.transform;
        return transform;
    }

    protected virtual void OnOrderAndCoverCheck() { }

    protected virtual void OnDestroy()
    {
        if(!isBeingDestroyed)
        {
            if (PopUpBeingDestroyedEvent != null)
            {
                PopUpBeingDestroyedEvent(this);
            }

            SceneManager.sceneUnloaded -= OnNewSceneLoaded;
            isBeingDestroyed = true;
        }
    }

    private void UpdateName()
    {
        gameObject.name = setName + nameExtenstisetNameAddon;
        if(coverBackground != null)
        {
            coverBackground.name = setName + "(C)" + nameExtenstisetNameAddon;
        }
    }

    private void DestroyPopUpObject()
    {
        Destroy(gameObject);
        if (coverBackground != null)
            Destroy(coverBackground.gameObject);
    }
}
