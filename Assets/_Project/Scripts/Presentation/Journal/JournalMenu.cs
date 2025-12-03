using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Application.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Project.Scripts.Presentation.Journal
{
    [Serializable]
    public class JournalTabPage
    {
        public Toggle tab;
        public CanvasGroup page;
    }
    
    public class JournalMenu : MonoBehaviour
    {
        [SerializeField] private GameObject notesNewBadge;
        
        [Header("Tabs / Pages")]
        [SerializeField] private List<JournalTabPage> tabPages = new();

        [Header("Current Page")]
        [SerializeField] private int currentIndex = 0;

        [Header("Event")]
        public UnityEvent<int> onPageIndexChanged;

        public bool IsMemoryTabActive => currentIndex == 0;
        public bool IsNotesTabActive => currentIndex == 1;
        

        private void Awake()
        {
            var toggleGroup = GetComponent<ToggleGroup>();

            for (int i = 0; i < tabPages.Count; i++)
            {
                int capturedIndex = i;
                var entry = tabPages[i];
                var tab = entry.tab;

                if (toggleGroup != null)
                {
                    tab.group = toggleGroup;
                }

                tab.onValueChanged.AddListener(isOn =>
                {
                    if (isOn)
                    {
                        OpenPage(capturedIndex);
                    }
                });
            }
            
            UIEvents.OnJournalNotesChanged += HandleNotesChanged;
        }

        private void OnEnable()
        {
            currentIndex = Mathf.Clamp(currentIndex, 0, tabPages.Count - 1);
            tabPages[currentIndex].tab.isOn = true; 
        }


        private void OnDestroy()
        {
            UIEvents.OnJournalNotesChanged -= HandleNotesChanged;
            
            foreach (var entry in tabPages.Where(entry => entry?.tab != null))
            {
                entry.tab.onValueChanged.RemoveAllListeners();
            }
        }
        
        private void HandleNotesChanged()
        {
            notesNewBadge.SetActive(true);
        }

        private void OpenPage(int index)
        {
            int clamped = Mathf.Clamp(index, 0, tabPages.Count - 1);
            currentIndex = clamped;

            for (int i = 0; i < tabPages.Count; i++)
            {
                var entry = tabPages[i];
              
                var page = entry.page;
                bool active = (i == currentIndex);

                if (tabPages[i].tab.isOn != active)
                {
                    tabPages[i].tab.SetIsOnWithoutNotify(active); 
                }

                page.alpha = active ? 1 : 0;
                page.interactable = active;
                page.blocksRaycasts = active;

            }
            
            if (currentIndex == 1)
            {
                notesNewBadge.SetActive(false);
            }
            
            UIEvents.RaiseJournalTabChanged(currentIndex);
            onPageIndexChanged?.Invoke(currentIndex);
        }

        public void JumpToPage(int index)
        {
            int clamped = Mathf.Clamp(index, 0, tabPages.Count - 1);
            var entry = tabPages[clamped];
            entry.tab.isOn = true;
        }
        
        public void SwitchToNextTab()
        {
            currentIndex = (currentIndex + 1) % tabPages.Count;
            SelectTab(currentIndex);
        }

        public void SwitchToPreviousTab()
        {
            currentIndex--;
            if (currentIndex < 0) currentIndex = tabPages.Count - 1;
            SelectTab(currentIndex);
        }

        private void SelectTab(int index)
        {
            int clamped = Mathf.Clamp(index, 0, tabPages.Count - 1);
            if (clamped < tabPages.Count && tabPages[clamped]?.tab != null)
            {
                tabPages[clamped].tab.isOn = true;
            }
        }

    }

}
