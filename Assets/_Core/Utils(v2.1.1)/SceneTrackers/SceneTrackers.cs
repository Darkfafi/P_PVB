using System.Collections.Generic;
using UnityEngine;

namespace Ramses.SceneTrackers
{
    public class SceneTrackers : MonoBehaviour
    {
        private List<ISceneTracker> systems = new List<ISceneTracker>();

        private bool initialized = false;

        public T GetSceneTracker<T>() where T : class, ISceneTracker
        {
            AddTrackers();
            for (int i = systems.Count - 1; i >= 0; i--)
            {
                if (systems[i].GetType().IsAssignableFrom(typeof(T)))
                {
                    return (T)systems[i];
                }
            }
            return null;
        }

        protected virtual void Awake()
        {
            AddTrackers();
        }

        protected virtual void OnDestroy()
        {
            for (int i = systems.Count - 1; i >= 0; i--)
            {
                systems[i].SystemDestroyCall();
            }
        }

        protected void SetNonMonobehaviourSceneSystems(params ISceneTracker[] nonBehaviourSystems)
        {
            for (int i = 0; i < nonBehaviourSystems.Length; i++)
            {
                if (!systems.Contains(nonBehaviourSystems[i]))
                {
                    systems.Add(nonBehaviourSystems[i]);
                }
            }

            bool fs = true;
            for (int i = 0; i < nonBehaviourSystems.Length; i++)
            {
                if (fs)
                {
                    nonBehaviourSystems[i].SystemAwakeCall();
                    if (i == nonBehaviourSystems.Length - 1)
                    {
                        i = -1;
                        fs = false;
                    }
                }
                else
                {
                    nonBehaviourSystems[i].SystemStartCall();
                }
            }
        }

        private void AddTrackers()
        {
            if(!initialized)
            {
                initialized = true;
                List<ISceneTracker> ts = new List<ISceneTracker>(GetComponents<ISceneTracker>());
                ts.AddRange(GetComponentsInChildren<ISceneTracker>());

                for (int i = 0; i < ts.Count; i++)
                {
                    if (!systems.Contains(ts[i]))
                    {
                        systems.Add(ts[i]);
                    }
                }

                bool fs = true;
                for (int i = 0; i < systems.Count; i++)
                {
                    if (systems[i].GetType().IsSubclassOf(typeof(MonoBehaviour)))
                    {
                        if (fs)
                        {
                            systems[i].SystemAwakeCall();
                            if (i == systems.Count - 1)
                            {
                                i = -1;
                                fs = false;
                            }
                        }
                        else
                        {
                            systems[i].SystemStartCall();
                        }
                    }
                }
            }
        }
    }

    public interface ISceneTracker
    {
        void SystemAwakeCall();
        void SystemStartCall();
        void SystemDestroyCall();
        void SystemRequested();
    }
}