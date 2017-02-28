using DG.Tweening;
using UnityEngine;

public static class CanvasAnimations
{
    public enum HorizontalOrientations
    {
        Left,
        Right
    }

    public enum VerticalOrientation
    {
        Up,
        Down
    }

    //  -------------------- Side Animations Horizontal Start

    public static Tweener MoveInHorSide(Transform transformToCastAndMove, float movementSpeed, HorizontalOrientations moveInFrom, Ease ease = Ease.Unset)
    {
        return MoveInHorSide((RectTransform)transformToCastAndMove, movementSpeed, moveInFrom, ease);
    }

    public static Tweener MoveOutHorSide(Transform transformToCastAndMove, float movementSpeed, HorizontalOrientations moveOutTo, Ease ease = Ease.Unset)
    {
        return MoveOutHorSide((RectTransform)transformToCastAndMove, movementSpeed, moveOutTo, ease);
    }

    public static Tweener MoveInHorSide(RectTransform transformToMove, float movementSpeed, HorizontalOrientations moveInFrom, Ease ease = Ease.Unset)
    {
        Vector2 spawnPosition = transformToMove.anchoredPosition;
        Vector2 outsidePos = spawnPosition;
        float widthCalculationObject = transformToMove.rect.width;

        outsidePos.x = GetSelfParentPosition(transformToMove, moveInFrom);

        if (moveInFrom == HorizontalOrientations.Right)
        {
            outsidePos.x += widthCalculationObject * transformToMove.pivot.x;
        }
        else
        {
            outsidePos.x -= widthCalculationObject * (1 - transformToMove.pivot.x);
        }

        transformToMove.anchoredPosition = outsidePos;

        return transformToMove.DOAnchorPosX(spawnPosition.x, movementSpeed).SetEase(ease);
    }

    public static Tweener MoveOutHorSide(RectTransform transformToMove, float movementSpeed, HorizontalOrientations moveOutTo, Ease ease = Ease.Unset)
    {
        Vector2 outsidePos = transformToMove.anchoredPosition;
        float widthCalculationObject = transformToMove.rect.width;

        outsidePos.x = GetSelfParentPosition(transformToMove, moveOutTo);

        if (moveOutTo == HorizontalOrientations.Right)
        {
            outsidePos.x += widthCalculationObject * transformToMove.pivot.x;
        }
        else
        {
            outsidePos.x -= widthCalculationObject * (1 - transformToMove.pivot.x);
        }


        return transformToMove.DOAnchorPosX(outsidePos.x, movementSpeed).SetEase(ease);
    }

    //  -------------------- Vertical

    public static Tweener MoveInVertSide(Transform transformToCastAndMove, float movementSpeed, VerticalOrientation moveInFrom, Ease ease = Ease.Unset)
    {
        return MoveInVertSide((RectTransform)transformToCastAndMove, movementSpeed, moveInFrom, ease);
    }

    public static Tweener MoveOutVertSide(Transform transformToCastAndMove, float movementSpeed, VerticalOrientation moveOutTo, Ease ease = Ease.Unset)
    {
        return MoveOutVertSide((RectTransform)transformToCastAndMove, movementSpeed, moveOutTo, ease);
    }

    public static Tweener MoveInVertSide(RectTransform transformToMove, float movementSpeed, VerticalOrientation moveInFrom, Ease ease = Ease.Unset)
    {
        Vector2 spawnPosition = transformToMove.anchoredPosition;
        Vector2 outsidePos = spawnPosition;

        float heightCalculationObject = (transformToMove.rect.height);

        outsidePos.y = GetSelfParentPosition(transformToMove, moveInFrom);
        if (moveInFrom == VerticalOrientation.Up)
        {
            outsidePos.y += heightCalculationObject * transformToMove.pivot.y;
        }
        else
        {
            outsidePos.y -= heightCalculationObject * (1 - transformToMove.pivot.y);
        }

        transformToMove.anchoredPosition = outsidePos;

        return transformToMove.DOAnchorPosY(spawnPosition.y, movementSpeed).SetEase(ease);
    }

    public static Tweener MoveOutVertSide(RectTransform transformToMove, float movementSpeed, VerticalOrientation moveOutTo, Ease ease = Ease.Unset)
    {
        Vector2 outsidePos = transformToMove.anchoredPosition;
        float heightCalculationObject = (transformToMove.rect.height);

        outsidePos.y = GetSelfParentPosition(transformToMove, moveOutTo);
        if (moveOutTo == VerticalOrientation.Up)
        {
            outsidePos.y += heightCalculationObject * transformToMove.pivot.y;
        }
        else
        {
            outsidePos.y -= heightCalculationObject * (1 - transformToMove.pivot.y);
        }
        return transformToMove.DOAnchorPosY(outsidePos.y, movementSpeed).SetEase(ease);
    }

    public static float GetSelfParentPosition(RectTransform self, VerticalOrientation orientation)
    {
        RectTransform parentTransform = ((RectTransform)self.parent.transform);
        float heighestPoint = self.anchorMax.y * parentTransform.rect.height;
        float lowestPoint = self.anchorMin.y * parentTransform.rect.height;
        if (orientation == VerticalOrientation.Up)
            return parentTransform.rect.height - heighestPoint;
        else
            return 0 - lowestPoint;
    }

    public static float GetSelfParentPosition(RectTransform self, HorizontalOrientations orientation)
    {
        RectTransform parentTransform = ((RectTransform)self.parent.transform);
        float rightPoint = self.anchorMax.x * parentTransform.rect.width;
        float leftPoint = self.anchorMin.x * parentTransform.rect.width;
        if (orientation == HorizontalOrientations.Right)
            return parentTransform.rect.width - rightPoint;
        else
            return 0 - leftPoint;
    }
}
