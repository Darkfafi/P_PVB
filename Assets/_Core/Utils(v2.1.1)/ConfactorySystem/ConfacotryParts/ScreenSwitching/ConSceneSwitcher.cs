using UnityEngine;
using System.Collections;
using Ramses.Confactory;
using System;
using UnityEngine.SceneManagement;
using Ramses.Confactory.Addons;

public class ConSceneSwitcher : IConfactory, IConSceneSwitchUser, IConStructUser
{
	public delegate void SceneSwitcherHandler();
	public event SceneSwitcherHandler FullBlackEvent;
	public event SceneSwitcherHandler BlackClearedEvent;

	ScreenTransitionObject transitionObject;
	private string nextSceneName = "NO_SCENE";
	private bool fakeSwitch = false;


	public void SwitchScreen(string sceneName)
	{
		nextSceneName = sceneName;
        transitionObject.FadeIn();
    }

	public void FakeSwitchScreen()
	{
		nextSceneName = "Fake";
        fakeSwitch = true;
        transitionObject.FadeIn();
	}

	public void ConClear()
	{
		transitionObject.FadeInCompleteEvent -= FadeInComplete;
		transitionObject.FadeOutCompleteEvent -= FadeOutComplete;
		GameObject.Destroy(transitionObject.gameObject);
	}

	private void FadeInComplete()
	{
		if (!fakeSwitch)
		{
			SceneManager.LoadScene(nextSceneName);
		}else
		{
			fakeSwitch = false;
			OnConSceneSwitched(SceneManager.GetActiveScene().name, SceneManager.GetActiveScene().name);
        }
		nextSceneName = "NO_SCENE";

		if(FullBlackEvent != null)
		{
			FullBlackEvent();
        }
	}

	private void FadeOutComplete()
	{
		if (BlackClearedEvent != null)
		{
			BlackClearedEvent();
		}
	}

	public void OnConSceneSwitched(string oldScene, string newScene)
	{
		transitionObject.FadeOut();
	}

	public IConfactory ConStruct(IConfactoryFinder confactoryFinder)
	{
		ScreenTransitionObject stoResource = Resources.Load<ScreenTransitionObject>("UI/TransitionScreen");
		if (stoResource == null)
		{
			Debug.LogWarning("No TransitionScreen found at 'Resources/UI/TransitionScreen'. Using Default screen instead");
			stoResource = Resources.Load<ScreenTransitionObject>("ConfactoryResources/ScreenSwitching/TransitionScreen");
		}

		transitionObject = GameObject.Instantiate<ScreenTransitionObject>(stoResource);
		ConfactoryTools.SetObjectWithConGameObjectSettings(transitionObject.gameObject);
		transitionObject.FadeInCompleteEvent -= FadeInComplete;
		transitionObject.FadeInCompleteEvent += FadeInComplete;


		transitionObject.FadeOutCompleteEvent -= FadeOutComplete;
		transitionObject.FadeOutCompleteEvent += FadeOutComplete;
		return null;
	}
}
