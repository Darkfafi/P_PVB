using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
    public static T RequireComponent<T>(this GameObject myObject) where T : Component
    {
        T c = myObject.GetComponent<T>();
        if (c == null)
            return myObject.AddComponent<T>();

        return c;
    }
	
}
