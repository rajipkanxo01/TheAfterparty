using _Project.Scripts.Application.Badge;
using _Project.Scripts.Application.Player;
using _Project.Scripts.Application.Core;
using UnityEngine;

namespace _Project.Scripts.Presentation.Badge
{
    public class BadgeInitializer : MonoBehaviour
    {
        [Header("Badge Setup")]
        [SerializeField] private GameObject badgePrefab;
        [SerializeField] private Transform spawnPoint;

        private BadgeManager _manager;

        private void Start()
        {
            var profile = ServiceLocater.GetService<PlayerProfile>();
            _manager = new BadgeManager(profile, badgePrefab, spawnPoint.position, spawnPoint.rotation);
        }

        private void OnDestroy()
        {
            _manager?.Cleanup();
        }
    }
}