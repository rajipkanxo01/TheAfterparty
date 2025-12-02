using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Player;
using _Project.Scripts.Data.Memory;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Presentation.Journal.MemoryTab
{
    public class MemoryPageController : MonoBehaviour
    {
        [SerializeField] private List<JournalMemorySlot> memorySlots;

        private PlayerProfile _playerProfile;
        private JournalMenu _menu;
        private MemoryDatabase _memoryDatabase;
        public int currentIndex = 0;


        private void OnEnable()
        {
            _menu = GetComponentInParent<JournalMenu>();
            if (_menu is null)
            {
                Debug.LogWarning("PageController: JournalMenu not found in Start.");
            }
            
            _playerProfile = ServiceLocater.GetService<PlayerProfile>();
            if (_playerProfile == null)
            {
                Debug.LogWarning("PageController: PlayerProfile not found in Awake.");
            }

            _memoryDatabase = ServiceLocater.GetService<MemoryDatabase>();
            if (_memoryDatabase == null)
            {
                Debug.LogWarning("PageController: MemoryDatabase not found in OnEnable.");
            }
            
            RefreshSlots();
            InitializeSelection();

            if (_menu is not null)
            {
                _menu.onPageIndexChanged.AddListener(OnTabChanged);
            }
        }

        private void OnDisable()
        {
            if (_menu != null)
            {
                _menu.onPageIndexChanged.RemoveListener(OnTabChanged);
            }
        }

        private void OnTabChanged(int index)
        {
            if (gameObject.activeInHierarchy)
            {
                RefreshSlots();
            }
        }
        
        private void RefreshSlots()
        { 
            Debug.Log("MemoryPageController: Refreshing memory slots...");
            foreach (var slot in memorySlots.Where(s => s != null))
            {
                bool unlocked = _playerProfile.HasUnlockedMemory(slot.MemoryID);

                if (unlocked)
                {
                    slot.SetUnlocked();
                }
                else
                {
                    slot.SetLocked();
                }
            }
        }

        private void InitializeSelection()
        {
            // Find the first unlocked memory that is not the current scene
            for (int i = 0; i < memorySlots.Count; i++)
            {
                if (IsUnlocked(i) && !IsCurrentScene(i))
                {
                    currentIndex = i;
                    UpdateHighlight();
                    return;
                }
            }
            
            // If no selectable memories, set to 0 and don't highlight anything
            currentIndex = 0;
            UpdateHighlight();
        }
        
        public void MoveSelection(int dir)
        {
            if (memorySlots == null || memorySlots.Count == 0) return;
            
            int searchIndex = currentIndex;
            
            while (true)
            {
                searchIndex += dir;
                
                if (searchIndex < 0 || searchIndex >= memorySlots.Count)
                {
                    break;
                }
                
                if (IsUnlocked(searchIndex) && !IsCurrentScene(searchIndex))
                {
                    currentIndex = searchIndex;
                    break;
                }
            }
            
            UpdateHighlight();
        }

        private void UpdateHighlight()
        {
            for (int i = 0; i < memorySlots.Count; i++)
            {
                bool highlight = (i == currentIndex) && IsUnlocked(i) && !IsCurrentScene(i);
                memorySlots[i].SetHighlighted(highlight);
            }
        }
        
        public void SelectCurrent()
        {
            if (IsUnlocked(currentIndex) && !IsCurrentScene(currentIndex))
            {
                memorySlots[currentIndex].LoadMemory();
            }
        }
        
        private bool IsUnlocked(int index)
        {
            return _playerProfile.HasUnlockedMemory(memorySlots[index].MemoryID);
        }

        private bool IsCurrentScene(int index)
        {
            if (_memoryDatabase == null) return false;
            
            var memory = _memoryDatabase.GetById(memorySlots[index].MemoryID);
            if (memory == null) return false;
            
            return SceneManager.GetActiveScene().name == memory.sceneName;
        }

    }
}
