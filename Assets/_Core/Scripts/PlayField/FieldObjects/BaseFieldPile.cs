using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public abstract class BaseFieldPile<T> : MonoBehaviour where T : class
{
    public delegate void GrabInfoHandler(GamePlayer player, T objectGrabbed);
    public event GrabInfoHandler VisualObjectArrivedEvent;
    public event GamePlayerHandler AllObjectsArrivedEvent;

    [Header("Options Pile")]
    [SerializeField]
    private float _objectArriveDuration = 0.4f;

    [Header("Requirements Pile")]
    [SerializeField]
    private GameObject _visualObjectPrefab;

    public void Grab(GamePlayer player)
    {
        Grab(player, 1);
    }

    public void Grab(GamePlayer player, int amount)
    {
        GrabLoop(player, amount, 1);
    }

    protected abstract T ObjectGrabbing();

    private void GrabLoop(GamePlayer player, int amount, int cardCount)
    {
        InternalGrab(player, amount).OnComplete(
        () => {
            if (cardCount < amount)
            {
                GrabLoop(player, amount, (cardCount + 1));
            }
            else
            {
                if (AllObjectsArrivedEvent != null)
                {
                    AllObjectsArrivedEvent(player);
                }
            }
        });
    }

    private Tweener InternalGrab(GamePlayer player, int amount)
    {
        T objectGrabbing = ObjectGrabbing();
        GameObject visualObject = Instantiate(_visualObjectPrefab);
        visualObject.transform.position = transform.position;
        Vector2 destination = Ramses.SceneTrackers.SceneTrackersFinder.Instance.GetSceneTracker<PlayfieldST>().Playfield.GetCornerByFaction(player.FactionType).transform.position;
        Vector2 dir = (destination - new Vector2(visualObject.transform.position.x, visualObject.transform.position.y)).normalized;
        destination += (dir * 1.1f);

        visualObject.transform.DOMove(destination, _objectArriveDuration).SetEase(Ease.InExpo).OnComplete(
        () =>
        {
            ObjectArrived(player, objectGrabbing);
            Destroy(visualObject.gameObject);
        });
        return visualObject.transform.DORotate(new Vector3(0, 0, (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) - 90f), _objectArriveDuration * 0.5f).SetDelay(_objectArriveDuration * 0.5f);
    }

    private void ObjectArrived(GamePlayer player, T objectGrabbing)
    {
        if (VisualObjectArrivedEvent != null)
            VisualObjectArrivedEvent(player, objectGrabbing);
    }
}
