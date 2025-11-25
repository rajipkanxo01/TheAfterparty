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
            _menu = GetComponentInParent<JournalMenu>();
            
            _playerProfile = ServiceLocater.GetService<PlayerProfile>();
            if (_playerProfile == null)
            {
                Debug.LogWarning("PageController: PlayerProfile not found in Awake.");
            }
            
            if (_menu != null)
            {
                _menu.onPageIndexChanged.AddListener(OnPageActivated);
            }
            
            UIEvents.OnJournalOpen += RefreshSlots;
        }

        private void OnDisable()
        {
            if (_menu != null)
            {
                _menu.onPageIndexChanged.RemoveListener(OnPageActivated);
            }
        }

        private void OnDestroy()
        {
            UIEvents.OnJournalOpen -= RefreshSlots;
        }
        
        private void OnPageActivated(int index)
        {
            RefreshSlots();
        }
        
        private void RefreshSlots()
        {
            Debug.Log("PageController: Refreshing memory slots...");

            if (_playerProfile == null)
            {
                Debug.LogError("PageController: Cannot refresh; playerProfile is NULL.");
                return;
            }

            if (memorySlots == null || memorySlots.Count == 0)
            {
                Debug.LogWarning("PageController: No memory slots assigned.");
                return;
            }
            
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
