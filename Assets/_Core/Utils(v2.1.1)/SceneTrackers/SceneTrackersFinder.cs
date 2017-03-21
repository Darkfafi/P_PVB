using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ramses.SceneTrackers
{
    /// <summary>
    /// This class is the head system of the SceneTracking system.
    /// </summary>
    [DisallowMultipleComponent]
    public class SceneTrackersFinder : MonoBehaviour
    {
        public static SceneTrackersFinder Instance {
            get {
                if (_instance == null || sceneIndex != SceneManager.GetActiveScene().buildIndex)
                {
                    sceneIndex = SceneManager.GetActiveScene().buildIndex;
                    _instance = FindObjectOfType<SceneTrackersFinder>();
                    if (_instance == null)
                    {
                        Debug.LogError("No Object with component 'SceneTrackersFinder' found in this scene! Please add one to the scene if you want to use this system");
                    }
                }
                return _instance;
            }
        }

        private static SceneTrackersFinder _instance = null;

        private static int sceneIndex = -1;

        [SerializeField]
        private SceneTrackers[] sceneTrackers;

        public T GetSceneTracker<T>() where T : class, ISceneTracker
        {
            T system = null;
            for (int i = 0; i < sceneTrackers.Length; i++)
            {
                system = sceneTrackers[i].GetSceneTracker<T>();
                if (system != null)
                {
                    system.SystemRequested();
                    return system;
                }
            }
            Debug.LogWarning("Could not find Tracker of Type: " + typeof(T).ToString() + ".", gameObject);
            return null;
        }

        protected void Awake()
        {
            if (sceneTrackers == null || sceneTrackers.Length == 0)
                Debug.LogError("No SceneTrackers linked to SceneTrackersFinder", gameObject);
        }
    }
}