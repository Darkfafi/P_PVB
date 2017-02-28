using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
namespace Ramses.TabContentDisplay
{
    [RequireComponent(typeof(Button), typeof(CanvasGroup))]
    public class ContentTab : MonoBehaviour
    {
        public delegate void ContentTabHandler(ContentTab tab);
        public event ContentTabHandler ContentTabSelectToggleEvent;

        public bool IsSelected { get; private set; }
        public string TabName { get; private set; }

        [SerializeField]
        private Image tabIcon;

        private Button tabButton;
        private CanvasGroup canvasGroup;


        private bool initialized = false;

        public void Initialize(string tabName, Sprite icon)
        {
            if (!initialized)
            {
                IsSelected = true;
                ToggleSelected(false);

                TabName = tabName;
                tabIcon.sprite = icon;
                initialized = true;
            }
        }

        public void ToggleSelected(bool selectedValue)
        {
            if (IsSelected != selectedValue)
            {
                canvasGroup.DOComplete();
                if (selectedValue)
                {
                    canvasGroup.DOFade(1f, 0.6f);
                }
                else
                {
                    canvasGroup.DOFade(0.6f, 0.6f);
                }

                IsSelected = selectedValue;

                if (ContentTabSelectToggleEvent != null)
                {
                    ContentTabSelectToggleEvent(this);
                }
            }
        }

        protected void Awake()
        {
            canvasGroup = gameObject.RequireComponent<CanvasGroup>();
            tabButton = gameObject.RequireComponent<Button>();
            tabButton.onClick.AddListener(OnTabButtonClicked);
        }

        protected void OnDestroy()
        {
            tabButton.onClick.RemoveListener(OnTabButtonClicked);
        }

        private void OnTabButtonClicked()
        {
            ToggleSelected(true);
        }
    }
}