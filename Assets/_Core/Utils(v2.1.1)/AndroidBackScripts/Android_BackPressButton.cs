using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Android_BackPressButton : MonoBehaviour {

    [SerializeField]
    private UnityEngine.UI.Button button;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            button.onClick.Invoke();
        }
    }
}
