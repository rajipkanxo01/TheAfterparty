using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Events;
using _Project.Scripts.Application.Player;
using UnityEngine;

namespace _Project.Scripts.Presentation.Journal.MemoryTab
{
    public class MemoryPageController : MonoBehaviour
    {
        [SerializeField] private List<JournalMemorySlot> memorySlots;

        private PlayerProfile _playerProfile;
        private JournalMenu _menu;

        private void Start()
        {
           
        }

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
            
            RefreshSlots();

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
            /*if (_playerProfile == null)
            {
                Debug.LogError("MemoryPageController: Cannot refresh; playerProfile is NULL.");
                return;
            }

            if (memorySlots == null || memorySlots.Count == 0)
            {
                Debug.LogWarning("MemoryPageController: No memory slots assigned.");
                return;
            }*/
            
            
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
    }
}
