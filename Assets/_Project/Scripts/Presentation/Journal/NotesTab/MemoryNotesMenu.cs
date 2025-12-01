using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Project.Scripts.Presentation.Journal.NotesTab
{
    [Serializable]
    public class MemoryNotePage
    {
        public Toggle button;
        public CanvasGroup content;
    }

    public class MemoryNotesMenu : MonoBehaviour
    {
        [Header("Memory Buttons + Pages")]
        [SerializeField] private List<MemoryNotePage> pages = new();

        [Header("Current Selection")]
        [SerializeField] private int currentIndex = 0;

        [Header("Events")]
        public UnityEvent<int> onMemorySelected;

        private void Awake()
        {
            var toggleGroup = GetComponent<ToggleGroup>();

            for (int i = 0; i < pages.Count; i++)
            {
                int captured = i;
                var entry = pages[i];

                if (toggleGroup != null)
                {
                    entry.button.group = toggleGroup;
                }

                entry.button.onValueChanged.AddListener(isOn =>
                {
                    if (isOn)
                        OpenPage(captured);
                });
            }
        }

        private void OnEnable()
        {
            currentIndex = Mathf.Clamp(currentIndex, 0, pages.Count - 1);
            pages[currentIndex].button.isOn = true;
        }

        private void OnDestroy()
        {
            foreach (var p in pages.Where(p => p?.button != null))
                p.button.onValueChanged.RemoveAllListeners();
        }

        private void OpenPage(int index)
        {
            int clamped = Mathf.Clamp(index, 0, pages.Count - 1);
            currentIndex = clamped;

            for (int i = 0; i < pages.Count; i++)
            {
                bool active = (i == currentIndex);

                var entry = pages[i];

                if (entry.button.isOn != active)
                    entry.button.SetIsOnWithoutNotify(active);

                var panel = entry.content;
                // panel.alpha = active ? 1 : 0;
                panel.interactable = active;
                panel.blocksRaycasts = active;
            }

            onMemorySelected?.Invoke(currentIndex);
        }

    }
}
