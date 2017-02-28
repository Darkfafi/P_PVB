using DG.Tweening;
using UnityEngine;

public static class CanvasAnimationsUI
{
    public static Tweener MoveInHorSide(Transform transformToCastAndMove, float movementSpeed, CanvasAnimations.HorizontalOrientations moveInFrom, Ease ease = Ease.Unset)
    {
        return CanvasAnimations.MoveInHorSide(transformToCastAndMove, movementSpeed, moveInFrom, ease).SetUpdate(true);
    }

    public static Tweener MoveOutHorSide(Transform transformToCastAndMove, float movementSpeed, CanvasAnimations.HorizontalOrientations moveOutTo, Ease ease = Ease.Unset)
    {
        return CanvasAnimations.MoveOutHorSide(transformToCastAndMove, movementSpeed, moveOutTo, ease).SetUpdate(true);
    }

    public static Tweener MoveInHorSide(RectTransform transformToMove, float movementSpeed, CanvasAnimations.HorizontalOrientations moveInFrom, Ease ease = Ease.Unset)
    {
        return CanvasAnimations.MoveInHorSide(transformToMove, movementSpeed, moveInFrom, ease).SetUpdate(true);
    }

    public static Tweener MoveOutHorSide(RectTransform transformToMove, float movementSpeed, CanvasAnimations.HorizontalOrientations moveOutTo, Ease ease = Ease.Unset)
    {
        return CanvasAnimations.MoveOutHorSide(transformToMove, movementSpeed, moveOutTo, ease).SetUpdate(true);
    }

    public static Tweener MoveInVertSide(Transform transformToCastAndMove, float movementSpeed, CanvasAnimations.VerticalOrientation moveInFrom, Ease ease = Ease.Unset)
    {
        return CanvasAnimations.MoveInVertSide(transformToCastAndMove, movementSpeed, moveInFrom, ease).SetUpdate(true);
    }

    public static Tweener MoveOutVertSide(Transform transformToCastAndMove, float movementSpeed, CanvasAnimations.VerticalOrientation moveOutTo, Ease ease = Ease.Unset)
    {
        return CanvasAnimations.MoveOutVertSide(transformToCastAndMove, movementSpeed, moveOutTo, ease).SetUpdate(true);
    }

    public static Tweener MoveInVertSide(RectTransform transformToMove, float movementSpeed, CanvasAnimations.VerticalOrientation moveInFrom, Ease ease = Ease.Unset)
    {
        return CanvasAnimations.MoveInVertSide(transformToMove, movementSpeed, moveInFrom, ease).SetUpdate(true);
    }

    public static Tweener MoveOutVertSide(RectTransform transformToMove, float movementSpeed, CanvasAnimations.VerticalOrientation moveOutTo, Ease ease = Ease.Unset)
    {
        return CanvasAnimations.MoveOutVertSide(transformToMove, movementSpeed, moveOutTo, ease).SetUpdate(true);
    }

    public static float GetSelfParentPosition(RectTransform self, CanvasAnimations.VerticalOrientation orientation)
    {
        return CanvasAnimations.GetSelfParentPosition(self, orientation);
    }

    public static float GetSelfParentPosition(RectTransform self, CanvasAnimations.HorizontalOrientations orientation)
    {
        return CanvasAnimations.GetSelfParentPosition(self, orientation);
    }

}
