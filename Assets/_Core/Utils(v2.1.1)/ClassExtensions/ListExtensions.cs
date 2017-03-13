using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ListExtensions
{
    public static int GetLoopIndex<T>(this List<T> list, int index)
    {
        if (list.Count > 0)
        {
            while (index > list.Count - 1)
            {
                index -= list.Count;
            }
            while (index < 0)
            {
                index += list.Count;
            }
        }
        else
        {
            index = 0;
        }
        return index;
    }

    public static int GetClampedIndex<T>(this List<T> list, int index)
    {
        if (index > list.Count - 1)
        {
            index = list.Count - 1;
        }
        else if (index < 0)
        {
            index = 0;
        }
        return index;
    }

    public static T GetLoop<T>(this List<T> list, int index)
    {
        return list[list.GetLoopIndex(index)];
    }

    public static T GetClamped<T>(this List<T> list, int index)
    {
        return list[list.GetClampedIndex(index)];
    }
}
