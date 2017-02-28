using UnityEngine;


namespace Ramses.TabContentDisplay
{
    public class TabContent : MonoBehaviour
    {

        public ContentTab LinkedTab { get; private set; }

        private bool initialized = false;

        public void Initialize(ContentTab tab)
        {
            if (!initialized)
            {
                LinkedTab = tab;
                LinkedTab.ContentTabSelectToggleEvent += OnContentTabSelectToggleEvent;
                initialized = true;
                InitializedAwake();
            }
        }

        protected virtual void OnContentTabSelectToggleEvent(ContentTab tab)
        {

        }

        protected virtual void OnDestroy()
        {
            if (LinkedTab != null)
                LinkedTab.ContentTabSelectToggleEvent -= OnContentTabSelectToggleEvent;
        }

        protected virtual void InitializedAwake() { }
    }
}
