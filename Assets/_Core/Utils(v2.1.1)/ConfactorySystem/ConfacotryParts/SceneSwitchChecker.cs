using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneSwitchChecker : MonoBehaviour {

	public delegate void SceneSwitchHandler(string previousScene, string currentScene);
	public event SceneSwitchHandler SceneSwitchEvent;

	public string PreviousSceneName { get; private set; }
	public string CurrentSceneName { get; private set; }

	private void Awake()
	{
		SceneManager.sceneLoaded -= OnSceneChanged;
		SceneManager.sceneLoaded += OnSceneChanged;
		PreviousSceneName = CurrentSceneName = SceneManager.GetActiveScene().name;
    }

	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= OnSceneChanged;
	}

	void OnSceneChanged(Scene newScene, LoadSceneMode mode)
	{
		PreviousSceneName = CurrentSceneName;
        CurrentSceneName = newScene.name;

		if (SceneSwitchEvent != null)
		{
			SceneSwitchEvent(PreviousSceneName, CurrentSceneName);
        }
	}
}
