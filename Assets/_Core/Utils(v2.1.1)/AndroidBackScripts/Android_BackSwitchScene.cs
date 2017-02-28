using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Android_BackSwitchScene : MonoBehaviour {

    [SerializeField]
    private string sceneName;

	private void Update ()
    {
		if(Input.GetKeyDown(KeyCode.Escape))
        {
            Ramses.Confactory.ConfactoryFinder.Instance.Get<ConSceneSwitcher>().SwitchScreen(sceneName);
        }
	}
}
