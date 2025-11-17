using _Project.Scripts.Application.Badge;
using _Project.Scripts.Application.Player;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Presentation.Objects;
using UnityEngine;

namespace _Project.Scripts.Presentation.Badge
{
    public class BadgeInitializer : MonoBehaviour
    {
        [Header("Badge Setup")]
        [SerializeField] private string chestId;
        [SerializeField] private GameObject badgePrefab;
        [SerializeField] private Transform spawnPoint;

        private BadgeManager _manager;

        private void Start()
        {
            var profile = ServiceLocater.GetService<PlayerProfile>();
            _manager = new BadgeManager(profile, badgePrefab, spawnPoint.position, spawnPoint.rotation);
        }

        private void OnEnable()
        {
            ChestEvents.OnChestOpened += HandleChestOpened;
        }

        private void OnDisable()
        {
            ChestEvents.OnChestOpened -= HandleChestOpened;
        }

        private void HandleChestOpened(string openedId)
        {
            if (openedId == chestId)
            {
                // Tell BadgeManager to spawn
                BadgeEvents.RaiseSpawnBadge();
            }
        }

        private void OnDestroy()
        {
            _manager?.Cleanup();
        }
    }
}