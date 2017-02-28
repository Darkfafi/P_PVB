using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Events;
public class Android_BackCallMethod : MonoBehaviour {

    [SerializeField]
    private UnityEvent method;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            method.Invoke();
        }
    }
}
