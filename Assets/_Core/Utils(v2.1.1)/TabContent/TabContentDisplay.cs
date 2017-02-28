using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Ramses.TabContentDisplay
{
    public class TabContentDisplay : MonoBehaviour
    {
        [SerializeField]
        private ContentTab contentTabPrefab;

        [SerializeField]
        private RectTransform contentTabContainer; // Tabs

        [SerializeField]
        private RectTransform contentContainer; // Display

        // --- 

        private TabDisplaySetting currentDisplaySetting = null;
        private List<ContentTab> createdContentTabs = new List<ContentTab>();

        private ContentTab currentlySelectedTab = null;
        private TabContent currentlyDisplayingContent = null;

        public void SetTabDisplaySetting(TabDisplaySetting setting)
        {
            UnsetTabDisplaySetting();
            currentDisplaySetting = setting;
            for (int i = 0; i < setting.TabDisplayInfos.Length; i++)
            {
                ContentTab t = CreateTab(setting.TabDisplayInfos[i]);
                t.ContentTabSelectToggleEvent += OnContentTabSelectToggleEvent;
                if (i == 0)
                {
                    t.ToggleSelected(true);
                }else
                {
                    t.ToggleSelected(false);
                }
            }
        }

        public void UnsetTabDisplaySetting()
        {
            if (currentDisplaySetting == null) { return; }
            DestroyAllTabs();
            currentDisplaySetting = null;
        }

        private void DestroyAllTabs()
        {
            ContentTab ct = null;
            for (int i = createdContentTabs.Count - 1; i >= 0; i--)
            {
                ct = createdContentTabs[i];
                ct.ContentTabSelectToggleEvent -= OnContentTabSelectToggleEvent;
                Destroy(ct.gameObject);
            }
            createdContentTabs.Clear();
        }

        private void OnContentTabSelectToggleEvent(ContentTab tab)
        {
            if (tab.IsSelected)
            {
                if (currentlySelectedTab != null)
                    currentlySelectedTab.ToggleSelected(false);
                currentlySelectedTab = tab;
                ShowDisplayContentOfTab(currentlySelectedTab);
            }
        }

        private ContentTab CreateTab(TabDisplayInfo tabDisplayInfo)
        {
            ContentTab tabInstance = GameObject.Instantiate(contentTabPrefab);
            tabInstance.transform.SetParent(contentTabContainer.transform, false);
            tabInstance.Initialize(tabDisplayInfo.TabName, tabDisplayInfo.TabIcon);
            return tabInstance;
        }

        private void ShowDisplayContentOfTab(ContentTab tab)
        {
            DestroyDisplayingContent();
            TabDisplayInfo? tabInfo = currentDisplaySetting.GetInfoByName(tab.TabName);
            if (tabInfo.HasValue)
            {
                currentlyDisplayingContent = GameObject.Instantiate<TabContent>(tabInfo.Value.displayContentPrefab);
                currentlyDisplayingContent.transform.SetParent(contentContainer.transform, false);
                currentlyDisplayingContent.Initialize(tab);
            }
        }

        private void DestroyDisplayingContent()
        {
            if (currentlyDisplayingContent == null) { return; }
            GameObject.Destroy(currentlyDisplayingContent.gameObject);
            currentlyDisplayingContent = null;
        }
    }

    // -------------- Display information -------------- \\

    public class TabDisplaySetting
    {
        public TabDisplayInfo[] TabDisplayInfos { get; private set; }

        public TabDisplaySetting(params TabDisplayInfo[] tabDisplayInfos)
        {
            TabDisplayInfos = tabDisplayInfos;
        }

        public TabDisplayInfo? GetInfoByName(string name)
        {
            TabDisplayInfo? tabReturn = null;
            for (int i = 0; i < TabDisplayInfos.Length; i++)
            {
                if (TabDisplayInfos[i].TabName == name)
                {
                    tabReturn = TabDisplayInfos[i];
                    break;
                }
            }

            return tabReturn;
        }
    }

    public struct TabDisplayInfo
    {
        [SerializeField]
        public string TabName;

        [SerializeField]
        public Sprite TabIcon;

        [SerializeField]
        public TabContent displayContentPrefab;

        public TabDisplayInfo(string name, Sprite icon, TabContent displayContentPrefab)
        {
            TabName = name;
            TabIcon = icon;
            this.displayContentPrefab = displayContentPrefab;
        }
    }
}